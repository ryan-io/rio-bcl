using System.Collections.Generic;
using UnityEngine;

namespace Procedural {
	public readonly struct RoomMeshData {
		public RoomMeshData(List<Vector3> vertices, List<int> triangles) {
			Vertices  = vertices;
			Triangles = triangles;
		}

		public List<Vector3> Vertices  { get; }
		public List<int>     Triangles { get; }
	}
}