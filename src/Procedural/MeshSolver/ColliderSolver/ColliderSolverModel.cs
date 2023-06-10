using System.Collections.Generic;
using UnityEngine;

namespace Procedural {
	public class ColliderSolverModel {
		public ColliderSolverModel(
			List<List<int>> outlines,
			GameObject colliderGameObject,
			List<Vector3> walkableVertices,
			LayerMask obstacleLayer, LayerMask boundaryLayer) {
			Outlines           = outlines;
			ColliderGameObject = colliderGameObject;
			WalkableVertices   = walkableVertices;
			ObstacleLayer      = obstacleLayer;
			BoundaryLayer      = boundaryLayer;
		}

		public List<List<int>> Outlines           { get; }
		public List<Vector3>   WalkableVertices   { get; }
		public GameObject      ColliderGameObject { get; }
		public LayerMask       ObstacleLayer      { get; }
		public LayerMask       BoundaryLayer      { get; }
	}
}