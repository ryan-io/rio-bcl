﻿using RIO.BCL;
using UnityBCL;
using UnityEngine;

namespace Procedural {
	public class RoomProcessor {
		public Vector2 GetInitialSpawnPosition() {
			var rooms = _roomData.Rooms;
			var count = rooms.Count;

			for (var i = 0; i < count; i++)
				if (rooms[i].IsMainRoom)
					return DeterminePosition(rooms[i]);

			return default;
		}

		public void Draw() {
			if (_roomData.Rooms.IsEmptyOrNull())
				return;

			var colorCounter = 0;
			var rooms        = _roomData.Rooms.ToArray();

			var roomCount = rooms.Length;

			for (var i = 0; i < roomCount; i++) {
				var edgeTiles = rooms[i].EdgeTiles;

				foreach (var edgeTile in edgeTiles) {
					var pos = _tileMonoModel.TileMapGameObjects.GridObject.CellToWorld(new Vector3Int(edgeTile.x,
						edgeTile.y, 0));
					DebugExt.DrawPoint(pos, _colors[colorCounter], 2f);
				}

				colorCounter++;
			}
		}

		Vector2 DeterminePosition(Room mainRoom) {
			_edgeTiles = mainRoom.EdgeTiles;
			var tiles = mainRoom.Tiles;
			var count = tiles.Count;

			var index = Random.Range(0, count - 1);
			var t     = tiles[index];

			return _tileMonoModel.TileMapGameObjects.GridObject.CellToWorld(new Vector3Int(t.x, t.y, 0));
		}

#region PLUMBING

		readonly Color[] _colors =
			{ Color.red, Color.green, Color.cyan, Color.yellow, Color.magenta, Color.blue, Color.white };

		public RoomProcessor(RoomProcessorDto dto) {
			_mapModel = dto.Model;
			_roomData = dto.RoomData;
			_weightedRandom = new WeightedRandom<int> {
				{ 0, 900 },
				{ 1, 1 }
			};
		}

		Region _edgeTiles;

		readonly ProceduralMapModel                    _mapModel;
		readonly ProceduralTileSolverMonobehaviorModel _tileMonoModel;
		readonly RoomData                              _roomData;
		readonly WeightedRandom<int>                   _weightedRandom;

#endregion
	}
}