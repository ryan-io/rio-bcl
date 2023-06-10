using UnityBCL;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Procedural {
	public static class TileLogic {
		public static TileMask SolveMask(int[,] map, int x, int y, bool isBoundary) {
			var bit = TileMask.None;

			if (isBoundary)
				return bit;

			if (IsFilled(map, x - 1, y + 1)) //Debug.Msg("Is NW");
				bit |= TileMask.NorthWest;

			if (IsFilled(map, x, y + 1)) //Debug.Msg("Is N");
				bit |= TileMask.North;

			if (IsFilled(map, x + 1, y + 1)) //Debug.Msg("Is NE");
				bit |= TileMask.NorthEast;

			if (IsFilled(map, x - 1, y)) //Debug.Msg("Is W");
				bit |= TileMask.West;

			if (IsFilled(map, x + 1, y)) //Debug.Msg("Is E");
				bit |= TileMask.East;

			if (IsFilled(map, x - 1, y - 1)) //Debug.Msg("Is SW");
				bit |= TileMask.SouthWest;

			if (IsFilled(map, x, y - 1)) //Debug.Msg("Is S");	
				bit |= TileMask.South;

			if (IsFilled(map, x + 1, y - 1)) //Debug.Msg("Is SE");
				bit |= TileMask.SouthEast;

			return bit;
		}

		public static bool IsFilled(int[,] map, int x, int y) => map[x, y] == 1;


		public static void SetTile(Tilemap map, TileBase tile, Vector3Int position) {
			if (map == null) {
				var log = new UnityLogging();
				log.Warning("The Tilemap map parameter, 'map', is null. Now returning.");

				return;
			}

			map.SetTile(position, tile);
		}
	}
}