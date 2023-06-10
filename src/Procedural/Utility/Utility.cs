using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Procedural {
	public static class Utility {
		public static bool IsBoundary(int mapWidth, int mapHeight, int x, int y) =>
			x == 0 || y == 0 || x == mapWidth - 1 || y == mapHeight - 1;

		public static bool HasTileAtPosition(Tilemap tilemap, Vector3Int position) => tilemap.HasTile(position);

		public static Vector3 GetTileWorldPosition(Tilemap tilemap, GridLayout layout, Vector3Int tileMapPosition) {
			if (!tilemap || !layout)
				return default;

			var cellPosition = layout.CellToWorld(tileMapPosition);

			return cellPosition;
		}

		public static void CreateTileLabel(TileSolverModel data, int x, int y, string text) {
			var grid           = data.TileObjects.GridObject;
			var tileSizeOffset = grid.cellSize;

			var position = new Vector3(
				-data.MapWidth  / 2f + x + tileSizeOffset.x / 2f,
				-data.MapHeight / 2f + y + tileSizeOffset.y / 2f,
				10);

			position *= data.CellSize;


			//TODO - a world txt function will need to be created if we want this functionality back in the future
			// var label = global::Utility.CreateWorldTextTMP(
			// 	text, null, 6, Color.yellow,
			// 	TextAlignmentOptions.Center, position);
			//
			// label.sortingLayerID = SortingLayer.NameToID(TileMapper.AboveSortingLayer);
			// label.gameObject.tag = TileMapper.Label;
		}

		public static void SetTileNullAtXY(int x, int y, KeyValuePair<TileMapType, Tilemap> map) {
			var position = new Vector3Int(x, y, 0);

			TileLogic.SetTile(map.Value, null, position);
		}

#if UNITY_EDITOR
		public static void ClearLogs() {
			var assembly = Assembly.GetAssembly(typeof(Editor));
			var type = assembly.GetType("UnityEditor.LogEntries");
			var method = type.GetMethod("Clear");
			method?.Invoke(new object(), null);
		}

#endif
	}
}