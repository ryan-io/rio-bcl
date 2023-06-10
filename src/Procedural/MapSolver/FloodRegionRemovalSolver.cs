using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityBCL;
using UnityEngine;

namespace Procedural {
	public class FloodRegionRemovalSolver : RegionRemovalSolver {
		const    string              RoomDataSuffix = "_RoomData";
		readonly MapConnectionSolver _mapConnectionSolver;
		readonly MapSolverModel      _model;

		List<Room> _rooms;

		public FloodRegionRemovalSolver(MapSolverModel model) {
			_model               = model;
			_mapConnectionSolver = new MapConnectionSolver(model);
			_rooms               = new List<Room>();
		}

		public override async UniTask<int[,]> Remove(int[,] map, CancellationToken token) {
			map = await CullWalls(map, token);
			map = await CullRooms(map, token);
			return map;
		}

		public RoomData SaveSolution(string hash, string seed) {
#if UNITY_STANDALONE || UNITY_EDITOR
			var log = new UnityLogging();
			log.Msg("Invoking save from the map process... ", size: 15, italic: true, bold: true,
#endif
				ctx: $"{Strings.ProcGen} Saving Rooms - SO Saver");
			var instance = ScriptableObject.CreateInstance<RoomData>();
			var date     = DateTime.UtcNow.ToString("HH-mm-ss_dd-M-yyyy");
			instance.Inject(_rooms, "seed-" + seed + "_date-" + date + "_hash-" + hash + "-" + RoomDataSuffix);
			return instance;
		}

		async UniTask<int[,]> CullWalls(int[,] map, CancellationToken token) {
			await UniTask.RunOnThreadPool(
				() => {
					var regions = GetRegions(map, 1).Result;

					foreach (var region in regions)
						if (region.Count < _model.WallRemovalThreshold)
							foreach (var tile in region)
								map[tile.x, tile.y] = 0;
				}, cancellationToken: token);

			return map;
		}

		async UniTask<int[,]> CullRooms(int[,] map, CancellationToken token) {
			await UniTask.RunOnThreadPool(
				async () => {
					var regions        = GetRegions(map, 0).Result;
					var survivingRooms = new List<Room>();

					foreach (var region in regions)
						if (region.Count < _model.RoomRemovalThreshold)
							foreach (var tile in region)
								map[tile.x, tile.y] = 1;
						else
							survivingRooms.Add(new Room(region, map));

					if (!survivingRooms.Any()) return;
					survivingRooms.Sort();
					survivingRooms[0].SetIsMainRoom(true);
					survivingRooms[0].SetIsAccessibleToMainRoomDirect(true);

					await _mapConnectionSolver.Connect(map, survivingRooms, token);
					_rooms = survivingRooms;
				}, cancellationToken: token).AsTask();

			return map;
		}

		Region GetRegionTiles(int[,] map, int startX, int startY) {
			var tiles    = new Region();
			var mapFlags = new bool[_model.MapWidth, _model.MapHeight];
			var tileType = map[startX, startY];

			var queue = new Queue<Vector2Int>();
			queue.Enqueue(new Vector2Int(startX, startY));
			mapFlags[startX, startY] = true;

			while (queue.Count > 0) {
				var tile = queue.Dequeue();
				tiles.Add(tile);
				for (var x = tile.x - 1; x <= tile.x + 1; x++) {
					for (var y = tile.y - 1; y <= tile.y + 1; y++)
						if (x >= 0 && x < _model.MapWidth && y >= 0 && y < _model.MapHeight
						    && (x == tile.x || y == tile.y))
							if (mapFlags[x, y] == false && map[x, y] == tileType) {
								mapFlags[x, y] = true;
								queue.Enqueue(new Vector2Int(x, y));
							}
				}
			}

			return tiles;
		}

		async Task<IEnumerable<Region>> GetRegions(int[,] map, int tileType) {
			var regions  = new List<Region>();
			var mapFlags = new bool[_model.MapWidth, _model.MapHeight];

			await UniTask.RunOnThreadPool(
				() => {
					for (var x = 0; x < _model.MapWidth; x++) {
						for (var y = 0; y < _model.MapHeight; y++)
							if (mapFlags[x, y] == false && map[x, y] == tileType) {
								var newRegion = GetRegionTiles(map, x, y);
								regions.Add(newRegion);
								foreach (var tile in newRegion)
									mapFlags[tile.x, tile.y] = true;
							}
					}
				});

			return regions;
		}
	}
}