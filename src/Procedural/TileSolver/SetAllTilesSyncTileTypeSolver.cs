using System;
using System.Collections.Concurrent;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityBCL;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Procedural {
	public class SetAllTilesSyncTileTypeSolver : TileTypeSolver {
		readonly TileSolverModel _model;

		public SetAllTilesSyncTileTypeSolver(TileSolverModel model) => _model = model;

		public TileHashset TileHashset => _model.TileHashset;

		// TODO - use of a concurrentbag will NOT guarantee tiles are place in the correct order.
		// this MAY be the reason I am seeing 'edge' cases where the incorrect tile is placed.
		public override async UniTask SetTiles(int[,] map, CancellationToken token) {
			var tasks = new ConcurrentBag<UniTask<TileData>>();

			for (var x = 0; x < _model.MapWidth; x++)
				for (var y = 0; y < _model.MapHeight; y++)
					await TileTaskCreator(map, x, y, tasks, token);

			try {
				await UniTask.WhenAll(tasks);
			}

			catch (AggregateException e) {
#if UNITY_STANDALONE || UNITY_EDITOR
				var log = new UnityLogging();
				foreach (var exception in e.Flatten().InnerExceptions) {
					log.Error($"Exception thrown when setting tiles. {exception.Message}");
#endif
#if UNITY_EDITOR
					EditorApplication.isPlaying = false;
#endif
				}
			}
		}

		UniTask TileTaskCreator(int[,] map, int x, int y, ConcurrentBag<UniTask<TileData>> tasks,
			CancellationToken token) {
			var isBoundary =
				Utility.IsBoundary(_model.MapWidth, _model.MapHeight, x, y);
			var bit = TileLogic.SolveMask(map, x, y, isBoundary);

			if (TileHandler.IsWall(bit))
				tasks.Add(FillTiles(map, x, y, ref bit));

			return new UniTask();
		}

		UniTask<TileData> FillTiles(int[,] map, int x, int y, ref TileMask bit) {
			var isFilled   = TileLogic.IsFilled(map, x, y);
			var location   = new Vector2Int(x, y);
			var isBoundary = Utility.IsBoundary(_model.MapWidth, _model.MapHeight, x, y);
			var isPocket   = false;

			TileMapper.FillGround(_model, x, y);

			var outLineRandom = _model.TileWeightDictionary["Outlines"];
			var value         = outLineRandom.Pop();
			var isLocalBorder = false;

			if (isFilled) {
				TileMapper.FillBoundary(_model, x, y);
				isLocalBorder = true;
			}

			else {
				TileMapper.FillAngles(_model, map, x, y);
				isPocket = TileMapper.FillPockets(_model, map, x, y);
			}

			var hasAll  = TileHandler.HasAllNeighbors(bit);
			var hasNone = TileHandler.HasNoNeighbors(bit);
			var data    = new TileData(location, bit, isBoundary, isLocalBorder);

			if (!hasAll && !hasNone && !isPocket && value == 1)
				RunFillOutlineLogic(x, y, ref data);

			AddToHashSet(data);

			return new UniTask<TileData>(data);
		}

		void RunFillOutlineLogic(int x, int y, ref TileData data) {
			var tileMapGameObjects = _model.TileObjects;
			var foregroundTable    = tileMapGameObjects.TileMapTable[TileMapType.ForegroundOne];
			var tile               = tileMapGameObjects.OutlineTileTable[TileId.Foliage.SouthOutline];
			TileMapper.FillOutlines(foregroundTable, tile, data, x, y - 1);
		}

		void AddToHashSet(TileData data) => _model.TileHashset.Add(data);
	}
}