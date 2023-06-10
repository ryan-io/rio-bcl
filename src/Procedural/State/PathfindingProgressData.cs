using Unity.Mathematics;
using UnityEngine.Tilemaps;

namespace Procedural {
	public readonly struct PathfindingProgressData {
		public PathfindingProgressData(int2 mapDimensions, float cellSize, Tilemap boundaryTilemap,
			Tilemap groundTilemap, TileHashset tileHashset, ProceduralController proceduralController, string seed,
			int iteration, string nameOfMap) {
			MapDimensions        = mapDimensions;
			CellSize             = cellSize;
			BoundaryTilemap      = boundaryTilemap;
			GroundTilemap        = groundTilemap;
			TileHashset          = tileHashset;
			ProceduralController = proceduralController;
			Seed                 = seed;
			Iteration            = iteration;
			NameOfMap            = nameOfMap;
		}

		public string               NameOfMap            { get; }
		public TileHashset          TileHashset          { get; }
		public Tilemap              BoundaryTilemap      { get; }
		public Tilemap              GroundTilemap        { get; }
		public int2                 MapDimensions        { get; }
		public float                CellSize             { get; }
		public ProceduralController ProceduralController { get; }
		public string               Seed                 { get; }
		public int                  Iteration            { get; }
	}
}