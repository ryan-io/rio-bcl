using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace Procedural {
	public class MapConnectionSolver {
		readonly MapSolverModel _model;
		//
		// Vector2 CoordinatesToWorldPoints(Vector2Int tile, MapDimensions dimensions) =>
		// 	new Vector2(
		// 		-dimensions.MapWidth  / 2f + 0.5f + tile.x,
		// 		-dimensions.MapHeight / 2f + 0.5f + tile.y);

		public MapConnectionSolver(MapSolverModel model) => _model = model;

		public async UniTask Connect(int[,] map, List<Room> rooms, CancellationToken token,
			bool forceAccessibility = false) {
			await UniTask.RunOnThreadPool(
				async () => {
					var roomListA = new List<Room>();
					var roomListB = new List<Room>();

					if (forceAccessibility) {
						foreach (var room in rooms) DetermineList(room, roomListB, roomListA);
					}
					else {
						roomListA = rooms;
						roomListB = rooms;
					}

					var bestDistance            = 0;
					var bestTileA               = new Vector2Int();
					var bestTileB               = new Vector2Int();
					var bestRoomA               = new Room();
					var bestRoomB               = new Room();
					var possibleConnectionFound = false;

					foreach (var roomA in roomListA) {
						if (!forceAccessibility) {
							possibleConnectionFound = false;
							if (roomA.ConnectedRooms.Count > 0) continue;
						}

						foreach (var roomB in roomListB) {
							if (roomA == roomB || roomA.IsConnected(roomB)) continue;
							for (var i = 0; i < roomA.EdgeTiles.Count; i++) {
								for (var j = 0; j < roomB.EdgeTiles.Count; j++) {
									if (token.IsCancellationRequested) break;

									var tileA = roomA.EdgeTiles[i];
									var tileB = roomB.EdgeTiles[j];
									var distance = (int)Mathf.Pow(tileA.x - tileB.x, 2) +
									               (int)Mathf.Pow(tileA.y - tileB.y, 2);

									if (distance < bestDistance || !possibleConnectionFound) {
										bestDistance            = distance;
										possibleConnectionFound = true;
										bestTileA               = tileA;
										bestTileB               = tileB;
										bestRoomA               = roomA;
										bestRoomB               = roomB;
									}
								}
							}
						}

						if (possibleConnectionFound && !forceAccessibility)
							CreatePassage(map, bestRoomA, bestRoomB, bestTileA, bestTileB);
					}

					if (possibleConnectionFound && forceAccessibility) {
						CreatePassage(map, bestRoomA, bestRoomB, bestTileA, bestTileB);
						await Connect(map, rooms, token, true); // ISSUE
					}

					if (!forceAccessibility)
						await Connect(map, rooms, token, true); // ISSUE
				}, cancellationToken: token);
		}

		static void DetermineList(Room room, List<Room> roomListB, List<Room> roomListA) {
			if (room.IsAccessibleToMainRoom)
				roomListB.Add(room);
			else
				roomListA.Add(room);
		}

		void CreatePassage(int[,] map, Room a, Room b, Vector2Int tileA, Vector2Int tileB) {
			Room.ConnectRooms(a, b);
			// Debug.DrawLine (CoordinatesToWorldPoints(tileA, dimensions), 
			// 	CoordinatesToWorldPoints(tileB, dimensions), Color.green, 100);
			var line = BresenhamDrawLine(tileA, tileB);

			foreach (var t in line) Draw(map, t, GetCorridorRadius(_model.CorridorWidth));
		}

		int GetCorridorRadius(Vector2 corridorWidth) {
			var r = new Random();
			return r.Next((int)corridorWidth.x, (int)corridorWidth.y);
		}


		void Draw(int[,] map, Vector2Int linePoint, int radius) {
			for (var x = -radius; x <= radius; x++) {
				for (var y = -radius; y <= radius; y++)
					if (x * x + y * y <= radius * radius) { // inside of the circle
						var drawX = linePoint.x + x;
						var drawY = linePoint.y + y;
						if (drawX >= 0 && drawX < _model.MapWidth && drawY >= 0 && drawY < _model.MapHeight)
							map[drawX, drawY] = 0;
					}
			}
		}

		Region BresenhamDrawLine(Vector2Int start, Vector2Int end) {
			var line = new Region();
			var x    = start.x;
			var y    = start.y;

			var dx = end.x - x;
			var dy = end.y - y;

			var inverted = false;

			var lateralStep   = Math.Sign(dx); // either -1 or +1, Bresenham's line drawing alg
			var gradientStep  = Math.Sign(dy);
			var largestDelta  = Mathf.Abs(dx);
			var smallestDelta = Mathf.Abs(dy);

			if (largestDelta < smallestDelta) {
				inverted      = true;
				largestDelta  = Mathf.Abs(dy);
				smallestDelta = Mathf.Abs(dx);
				lateralStep   = Math.Sign(dy);
				gradientStep  = Math.Sign(dx);
			}

			var gradientAccumulation = largestDelta / 2;

			for (var i = 0; i < largestDelta; i++) {
				line.Add(new Vector2Int(x, y));
				if (inverted) y += lateralStep;
				else x          += lateralStep;

				gradientAccumulation += smallestDelta;

				if (gradientAccumulation >= largestDelta) {
					if (inverted) x += gradientStep;
					else y          += gradientStep;
					gradientAccumulation -= largestDelta;
				}
			}

			return line;
		}
	}
}