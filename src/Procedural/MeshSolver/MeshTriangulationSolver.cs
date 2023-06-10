using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Procedural {
	public abstract class MeshTriangulationSolver {
		public          List<List<int>>                          Outlines   { get; protected set; }
		public          Mesh                                     SolvedMesh { get; protected set; }
		public abstract UniTask<Tuple<List<int>, List<Vector3>>> Triangulate(int[,] mapBorder, CancellationToken token);
	}
}