using UnityEngine;

namespace Procedural {
	public readonly struct MeshTriangulationSolverModel {
		public MeshFilter MeshFilter { get; }
		public float      SquareSize { get; }

		public MeshTriangulationSolverModel(
			ProceduralMapConfiguration mapConfig, MeshFilter meshFilter) {
			SquareSize = mapConfig.CellSize;
			MeshFilter = meshFilter;
		}
	}
}