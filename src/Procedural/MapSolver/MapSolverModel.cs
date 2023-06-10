using UnityEngine;

namespace Procedural {
	public readonly struct MapSolverModel {
		public int        MapWidth             { get; }
		public int        MapHeight            { get; }
		public int        SmoothingIterations  { get; }
		public int        WallRemovalThreshold { get; }
		public int        RoomRemovalThreshold { get; }
		public Vector2Int CorridorWidth        { get; }
		public int        WallFillPercentage   { get; }
		public int        UpperNeighborLimit   { get; }
		public int        LowerNeighborLimit   { get; }
		public int        Seed                 { get; }

		public MapSolverModel(ProceduralMapConfiguration configuration) {
			MapWidth             = configuration.MapWidth;
			MapHeight            = configuration.MapHeight;
			WallFillPercentage   = configuration.WallFillPercentage;
			SmoothingIterations  = configuration.SmoothingIterations;
			UpperNeighborLimit   = configuration.UpperNeighborLimit;
			LowerNeighborLimit   = configuration.LowerNeighborLimit;
			Seed                 = configuration.Seed.GetHashCode();
			WallRemovalThreshold = configuration.WallRemovalThreshold;
			RoomRemovalThreshold = configuration.RoomRemovalThreshold;
			CorridorWidth        = configuration.CorridorWidth;
		}
	}
}