using UnityEngine;

namespace Procedural {
	public readonly struct TileData {
		public Vector2Int Location        { get; }
		public TileMask   Bit             { get; }
		public bool       IsMapBoundary   { get; }
		public bool       IsLocalBoundary { get; }

		public TileData(Vector2Int location, TileMask bit, bool isMapBoundary, bool isLocalBoundary) {
			Location        = location;
			Bit             = bit;
			IsMapBoundary   = isMapBoundary;
			IsLocalBoundary = isLocalBoundary;
		}
	}
}