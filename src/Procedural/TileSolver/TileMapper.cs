using UnityEngine;
using UnityEngine.Tilemaps;

namespace Procedural {
	public static class TileMapper {
		public static readonly string Label             = "LabelDebug";
		public static readonly string AboveSortingLayer = "Above";

		public static void FillGround(TileSolverModel data, int x, int y) {
			var position = new Vector3Int(x, y, 0);

			if (!data.TileObjects.TileMapTable.ContainsKey(TileMapType.Ground))
				return;

			TileLogic.SetTile(
				data.TileObjects.TileMapTable[TileMapType.Ground],
				data.TileConfig.Ground,
				position);
		}

		public static void FillBoundary(TileSolverModel data, int x, int y) {
			var position = new Vector3Int(x, y, 0);

			if (!data.TileObjects.TileMapTable.ContainsKey(TileMapType.Boundary))
				return;

			TileLogic.SetTile(
				data.TileObjects.TileMapTable[TileMapType.Boundary],
				data.TileConfig.Boundary,
				position);
		}

		public static void FillAngles(TileSolverModel data, int[,] map, int x, int y) {
			if (data.TileConfig == null || data.TileConfig == null)
				return;

			if (Utility.IsBoundary(data.MapWidth, data.MapHeight, x, y))
				return;

			if (IsNorthWestTile(map, x, y))
				CreateNorthWestAngle(data, x, y);

			else if (IsNorthEastTile(map, x, y))
				CreateNorthEastAngle(data, x, y);

			else if (IsSouthWestAngle(map, x, y))
				CreateSouthWestAngle(data, x, y);

			else if (IsSouthEastAngle(map, x, y))
				CreateSouthEastAngle(data, x, y);
		}

		public static bool FillPockets(TileSolverModel data, int[,] map, int x, int y) {
			if (Utility.IsBoundary(data.MapWidth, data.MapHeight, x, y))
				return false;

			if (IsNorthPocket(map, x, y)) {
				CreateNorthPocket(data, x, y);

				return true;
			}

			if (IsSouthPocket(map, x, y)) {
				CreateSouthPocket(data, x, y);

				return true;
			}

			if (IsEastPocket(map, x, y)) {
				CreateEastPocket(data, x, y);

				return true;
			}

			if (IsWestPocket(map, x, y)) {
				CreateWestPocket(data, x, y);

				return true;
			}

			return false;
		}

		public static void FillOutlines(Tilemap tilemap, TileBase tile, TileData tileData, int x, int y) {
			if (!TileHandler.IsSouthOutline(tileData.Bit))
				return;

			var position = new Vector3Int(x, y, 0);
			TileLogic.SetTile(tilemap, tile, position);
		}

		static void CreateWestPocket(TileSolverModel data, int x, int y) {
			if (data.TileConfig.ShouldCreateLabels)
				Utility.CreateTileLabel(data, x, y, "W-P");

			TileLogic.SetTile(
				data.TileObjects.TileMapTable[TileMapType.Boundary],
				data.TileConfig.PocketWest,
				new Vector3Int(x, y, 0));
		}

		static void CreateEastPocket(TileSolverModel data, int x, int y) {
			if (data.TileConfig.ShouldCreateLabels)
				Utility.CreateTileLabel(data, x, y, "E-P");

			TileLogic.SetTile(
				data.TileObjects.TileMapTable[TileMapType.Boundary],
				data.TileConfig.PocketEast,
				new Vector3Int(x, y, 0));
		}

		static void CreateSouthPocket(TileSolverModel data, int x, int y) {
			if (data.TileConfig.ShouldCreateLabels)
				Utility.CreateTileLabel(data, x, y, "S-P");

			TileLogic.SetTile(
				data.TileObjects.TileMapTable[TileMapType.Boundary],
				data.TileConfig.PocketSouth,
				new Vector3Int(x, y, 0));
		}

		static void CreateNorthPocket(TileSolverModel data, int x, int y) {
			if (data.TileConfig.ShouldCreateLabels)
				Utility.CreateTileLabel(data, x, y, "N-P");

			TileLogic.SetTile(
				data.TileObjects.TileMapTable[TileMapType.Boundary],
				data.TileConfig.PocketNorth,
				new Vector3Int(x, y, 0));
		}

		static void CreateSouthEastAngle(TileSolverModel data, int x, int y) {
			if (data.TileConfig.ShouldCreateLabels)
				Utility.CreateTileLabel(data, x, y, "SE");

			TileLogic.SetTile(
				data.TileObjects.TileMapTable[TileMapType.Boundary],
				data.TileConfig.AngleSouthEast,
				new Vector3Int(x, y, 0));
		}

		static void CreateSouthWestAngle(TileSolverModel data, int x, int y) {
			if (data.TileConfig.ShouldCreateLabels)
				Utility.CreateTileLabel(data, x, y, "SW");

			TileLogic.SetTile(
				data.TileObjects.TileMapTable[TileMapType.Boundary],
				data.TileConfig.AngleSouthWest,
				new Vector3Int(x, y, 0));
		}

		static void CreateNorthEastAngle(TileSolverModel data, int x, int y) {
			if (data.TileConfig.ShouldCreateLabels)
				Utility.CreateTileLabel(data, x, y, "NE");

			TileLogic.SetTile(
				data.TileObjects.TileMapTable[TileMapType.Boundary],
				data.TileConfig.AngleNorthEast,
				new Vector3Int(x, y, 0));
		}

		static void CreateNorthWestAngle(TileSolverModel data, int x, int y) {
			if (data.TileConfig.ShouldCreateLabels)
				Utility.CreateTileLabel(data, x, y, "NW");

			TileLogic.SetTile(
				data.TileObjects.TileMapTable[TileMapType.Boundary],
				data.TileConfig.AngleNorthWest,
				new Vector3Int(x, y, 0));
		}

		static bool IsSouthEastAngle(int[,] map, int x, int y) =>
			!TileLogic.IsFilled(map, x - 1, y) &&
			(CaseFour(map, x, y)                || CaseEight(map, x, y) || CaseTwelve(map, x, y)
			 || CaseSouthEastEdgeOne(map, x, y) || CaseSouthEastEdgeTwo(map, x, y));

		static bool IsSouthWestAngle(int[,] map, int x, int y) =>
			(!TileLogic.IsFilled(map, x + 1, y) &&
			 (CaseThree(map, x, y)               || CaseSeven(map, x, y) || CaseEleven(map, x, y)
			  || CaseSouthWestEdgeOne(map, x, y) || CaseSouthWestEdgeTwo(map, x, y)))
			|| CaseSouthWestEdgeThree(map, x, y);

		static bool IsNorthEastTile(int[,] map, int x, int y) =>
			(!TileLogic.IsFilled(map, x - 1, y) &&
			 (CaseTwo(map, x, y)              || CaseSix(map, x, y) || CaseTen(map, x, y) ||
			  CaseNorthEastEdgeOne(map, x, y) ||
			  CaseNorthEastEdgeTwo(map, x, y))) ||
			CaseNorthEastEdgeThree(map, x, y);


		static bool IsNorthWestTile(int[,] map, int x, int y) =>
			!TileLogic.IsFilled(map, x + 1, y) &&
			(CaseOne(map, x, y)              || CaseFive(map, x, y) || CaseNine(map, x, y) ||
			 CaseNorthWestEdgeOne(map, x, y) || CaseNorthWestEdgeTwo(map, x, y));

		static bool IsNorthPocket(int[,] map, int x, int y) =>
			(CaseThree(map, x, y) && CaseFour(map, x, y)) ||
			CaseNorthPocketEdgeOne(map, x, y)             ||
			CaseNorthPocketEdgeTwo(map, x, y);

		static bool IsSouthPocket(int[,] map, int x, int y) =>
			(CaseOne(map, x, y) && CaseTwo(map, x, y)) || (CaseOne(map, x, y) && TileLogic.IsFilled(map, x + 1, y));

		static bool IsEastPocket(int[,] map, int x, int y) =>
			(CaseOne(map, x, y)   && CaseThree(map, x, y))              ||
			(CaseOne(map, x, y)   && TileLogic.IsFilled(map, x, y - 1)) ||
			(CaseThree(map, x, y) && TileLogic.IsFilled(map, x, y + 1));

		static bool IsWestPocket(int[,] map, int x, int y) =>
			(CaseTwo(map, x, y) && CaseFour(map, x, y)) ||
			CaseWestPocketEdgeOne(map, x, y)            ||
			CaseWestPocketEdgeTwo(map, x, y);

		static bool CaseWestPocketEdgeTwo(int[,] map, int x, int y) =>
			CaseFour(map, x, y) && TileLogic.IsFilled(map, x, y + 1);

		static bool CaseWestPocketEdgeOne(int[,] map, int x, int y) =>
			CaseFour(map, x, y) && TileLogic.IsFilled(map, x, y + 1) && TileLogic.IsFilled(map, x - 1, y + 1);

		static bool CaseOne(int[,] map, int x, int y) =>
			TileLogic.IsFilled(map, x - 1, y) && TileLogic.IsFilled(map, x - 1, y + 1) &&
			TileLogic.IsFilled(map, x,     y                               + 1);

		static bool CaseTwo(int[,] map, int x, int y) =>
			TileLogic.IsFilled(map, x, y + 1) && TileLogic.IsFilled(map, x + 1, y + 1) &&
			TileLogic.IsFilled(map, x    + 1,                            y);

		static bool CaseThree(int[,] map, int x, int y) =>
			TileLogic.IsFilled(map, x - 1, y) && TileLogic.IsFilled(map, x - 1, y - 1) &&
			TileLogic.IsFilled(map, x,     y                               - 1);

		static bool CaseFour(int[,] map, int x, int y) =>
			TileLogic.IsFilled(map, x + 1, y) && TileLogic.IsFilled(map, x + 1, y - 1) &&
			TileLogic.IsFilled(map, x,     y                               - 1);

		static bool CaseFive(int[,] map, int x, int y) =>
			CaseOne(map, x, y) && TileLogic.IsFilled(map, x + 1, y + 1) && TileLogic.IsFilled(map, x - 1, y + 1);

		static bool CaseSix(int[,] map, int x, int y) => CaseTwo(map, x, y) && TileLogic.IsFilled(map, x - 1, y + 1) &&
		                                                 TileLogic.IsFilled(map,                       x + 1, y - 1);

		static bool CaseSeven(int[,] map, int x, int y) =>
			CaseThree(map, x, y) && TileLogic.IsFilled(map, x - 1, y + 1) && TileLogic.IsFilled(map, x + 1, y - 1);

		static bool CaseNorthPocketEdgeOne(int[,] map, int x, int y) =>
			CaseThree(map, x, y) && TileLogic.IsFilled(map, x + 1, y + 1) && TileLogic.IsFilled(map, x + 1, y);

		static bool CaseNorthPocketEdgeTwo(int[,] map, int x, int y) =>
			CaseFour(map, x, y) && TileLogic.IsFilled(map, x - 1, y);

		static bool CaseEight(int[,] map, int x, int y) =>
			CaseFour(map, x, y) && TileLogic.IsFilled(map, x + 1, y + 1) && TileLogic.IsFilled(map, x - 1, y + 1);

		static bool CaseNine(int[,] map, int x, int y) =>
			TileLogic.IsFilled(map, x - 1, y - 1) &&
			TileLogic.IsFilled(map, x - 1, y)     &&
			TileLogic.IsFilled(map, x,     y + 1) &&
			TileLogic.IsFilled(map, x        + 1, y + 1);

		static bool CaseTen(int[,] map, int x, int y) =>
			TileLogic.IsFilled(map, x - 1, y + 1)    &&
			TileLogic.IsFilled(map, x,     y + 1)    &&
			TileLogic.IsFilled(map, x        + 1, y) &&
			TileLogic.IsFilled(map, x        + 1, y - 1);

		static bool CaseEleven(int[,] map, int x, int y) =>
			TileLogic.IsFilled(map, x - 1, y + 1) &&
			TileLogic.IsFilled(map, x - 1, y)     &&
			TileLogic.IsFilled(map, x,     y - 1) &&
			TileLogic.IsFilled(map, x        + 1, y - 1);

		static bool CaseTwelve(int[,] map, int x, int y) =>
			TileLogic.IsFilled(map, x + 1, y + 1) &&
			TileLogic.IsFilled(map, x + 1, y)     &&
			TileLogic.IsFilled(map, x,     y - 1) &&
			TileLogic.IsFilled(map, x        - 1, y - 1);

		static bool CaseNorthWestEdgeOne(int[,] map, int x, int y) =>
			TileLogic.IsFilled(map, x - 1, y)     &&
			TileLogic.IsFilled(map, x,     y + 1) &&
			TileLogic.IsFilled(map, x        + 1, y + 1);

		static bool CaseNorthWestEdgeTwo(int[,] map, int x, int y) =>
			CaseOne(map, x, y) && TileLogic.IsFilled(map, x + 1, y - 1);

		static bool CaseSouthEastEdgeOne(int[,] map, int x, int y) =>
			TileLogic.IsFilled(map, x + 1, y)     &&
			TileLogic.IsFilled(map, x,     y - 1) &&
			TileLogic.IsFilled(map, x        - 1, y - 1);

		static bool CaseSouthEastEdgeTwo(int[,] map, int x, int y) =>
			CaseFour(map, x, y) && TileLogic.IsFilled(map, x - 1, y + 1);

		static bool CaseSouthWestEdgeOne(int[,] map, int x, int y) =>
			TileLogic.IsFilled(map, x - 1, y + 1) && TileLogic.IsFilled(map, x - 1, y) &&
			TileLogic.IsFilled(map, x,     y - 1);

		static bool CaseSouthWestEdgeTwo(int[,] map, int x, int y) =>
			CaseThree(map, x, y) && TileLogic.IsFilled(map, x + 1, y + 1);

		static bool CaseSouthWestEdgeThree(int[,] map, int x, int y) =>
			TileLogic.IsFilled(map, x - 1, y) && TileLogic.IsFilled(map, x, y - 1);

		static bool CaseNorthEastEdgeOne(int[,] map, int x, int y) =>
			TileLogic.IsFilled(map, x, y + 1)    &&
			TileLogic.IsFilled(map, x    + 1, y) &&
			TileLogic.IsFilled(map, x    + 1, y - 1);

		static bool CaseNorthEastEdgeTwo(int[,] map, int x, int y) =>
			CaseTwo(map, x, y) && TileLogic.IsFilled(map, x - 1, y - 1);

		static bool CaseNorthEastEdgeThree(int[,] map, int x, int y) =>
			TileLogic.IsFilled(map, x + 1, y) && TileLogic.IsFilled(map, x, y + 1);
	}
}