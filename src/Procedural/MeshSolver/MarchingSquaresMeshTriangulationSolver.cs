using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Procedural {
	public class MarchingSquaresMeshTriangulationSolver : MeshTriangulationSolver {
		readonly HashSet<int>                        _checkedVertices;
		readonly MeshTriangulationSolverModel        _model;
		readonly SquareGrid                          _squareGrid;
		readonly Dictionary<int, List<MeshTriangle>> _triangleTracker;
		readonly TriangulationAlgorithm              _triangulationAlgorithm;

		readonly ProceduralMeshUVSolver _uvSolver;

		public MarchingSquaresMeshTriangulationSolver(MeshTriangulationSolverModel model, int[,] mapBorder) {
			_uvSolver               = new ProceduralMeshUVSolver(mapBorder);
			Outlines                = new List<List<int>>();
			_squareGrid             = new SquareGrid(mapBorder, model.SquareSize);
			_model                  = model;
			_checkedVertices        = new HashSet<int>();
			_triangleTracker        = new Dictionary<int, List<MeshTriangle>>();
			_triangulationAlgorithm = new TriangulationAlgorithm();
		}

		public override async UniTask<Tuple<List<int>, List<Vector3>>> Triangulate(int[,] mapBorder,
			CancellationToken token) {
			await UniTask.Yield();
			SetTriangles();
			SolveMesh();
			SetMeshMaterial();
			OutLineConnectionSolver.Solve(
				_triangulationAlgorithm.GetWalkableVertices, _checkedVertices, Outlines, _triangleTracker);

			var output = Tuple.Create(
				_triangulationAlgorithm.GetWalkableTriangles,
				_triangulationAlgorithm.GetWalkableVertices);

			return output;
		}

		void ClearLists() {
			Outlines.Clear();
			_checkedVertices.Clear();
			_triangleTracker.Clear();
		}

		void SetTriangles() {
			var xLength = _squareGrid.Squares.GetLength(0);
			var yLength = _squareGrid.Squares.GetLength(1);

			for (var x = 0; x < xLength; x++) {
				for (var y = 0; y < yLength; y++)
					_triangulationAlgorithm.TriangulateSquare(
						_squareGrid.Squares[x, y], _checkedVertices, _triangleTracker);
			}
		}

		void SolveMesh() {
			var mesh = new Mesh { name = "Procedural Cave Mesh" };
			_model.MeshFilter.mesh = mesh;

			var vertices = _triangulationAlgorithm.GetWalkableVertices;
			mesh.vertices  = vertices.ToArray();
			mesh.triangles = _triangulationAlgorithm.GetWalkableTriangles.ToArray();
			mesh.uv        = _uvSolver.CalculateUVs(vertices, _model.SquareSize);
			mesh.RecalculateNormals();

			SolvedMesh = mesh;
		}

		// What was the original intent for modifying a mesh renderer's material?
		// This method was present in the pre-refactored mesh solver
		void SetMeshMaterial() {
			// if (useMeshMaterial == Toggle.No)
			// 	_model.MeshRenderer.material = null;
		}
	}
}