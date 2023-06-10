using System.Collections.Generic;
using UnityBCL;
using UnityEngine;

namespace Procedural {
	public class ProceduralMeshUVSolver {
		readonly int[,] _proceduralMap;


		public ProceduralMeshUVSolver(int[,] proceduralMap) => _proceduralMap = proceduralMap;

		public Vector2[] CalculateUVs(IReadOnlyList<Vector3> vertices, float squareSize, int tilingMod = 1) {
			var uvs = new Vector2[vertices.Count];

			for (var i = 0; i < vertices.Count; i++) {
				var uvX = Mathf.InverseLerp(
					          -_proceduralMap.GetLength(0) / 2f * squareSize,
					          _proceduralMap.GetLength(0)  / 2f * squareSize,
					          vertices[i].x) * tilingMod;

				var uvY = Mathf.InverseLerp(
					          -_proceduralMap.GetLength(0) / 2f * squareSize,
					          _proceduralMap.GetLength(0)  / 2f * squareSize,
					          vertices[i].y) * tilingMod;

				uvs[i] = new Vector2(uvX, uvY);
			}

#if UNITY_EDITOR || UNITY_STANDALONE
			var log = new UnityLogging();
			log.Msg("UV's have been calculated.");
#endif
			return uvs;
		}
	}
}