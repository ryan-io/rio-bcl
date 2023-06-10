namespace Procedural {
	public static class TileHandler {
		// public static bool ContainsAllBits(TileMask comparer, params TileMask[] mask) {
		// 	var count = mask.Length;
		//
		// 	for (var i = 0; i < count; i++) {
		// 		var containsAll = comparer & mask[i];
		// 		
		// 		if (!containsAll)
		// 			return false;
		// 	}
		//
		// 	return true;
		// }

		const TileMask SouthOutline =
			TileMask.NorthWest | TileMask.North | TileMask.NorthEast | TileMask.West | TileMask.East;

		const TileMask AllMask = TileMask.NorthWest | TileMask.North     | TileMask.NorthEast | TileMask.West |
		                         TileMask.East      | TileMask.SouthWest | TileMask.South     | TileMask.SouthEast;

		public static bool IsSouthOutline(TileMask bit) => IsBit(bit, SouthOutline);

		public static bool HasNoNeighbors(TileMask bit) => bit == TileMask.None;

		public static bool HasAllNeighbors(TileMask bit) => bit == AllMask;

		public static bool IsWall(TileMask bit) => bit != AllMask;

		static bool IsBit(TileMask check, TileMask against) => check == against;

		public static bool ContainsAnyBits(TileMask comparer, params TileMask[] mask) {
			var count = mask.Length;

			for (var i = 0; i < count; i++) {
				var hasBit = (comparer & mask[i]) != 0;

				if (!hasBit)
					return false;
			}

			return true;
		}
	}

	public readonly struct ProceduralTileRule {
		public TileMask Rule { get; }

		public ProceduralTileRule(TileMask rule) => Rule = rule;
	}
}