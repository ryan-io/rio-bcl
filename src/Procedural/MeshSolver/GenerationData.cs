using System;
using UnityEngine;

namespace Procedural {
	[Serializable]
	public struct GenerationData {
		public GenerationData(Mesh generatedMesh, bool isCompleted, string meshSeed,
			RoomMeshDictionary generatedRoomMeshes) {
			GeneratedMesh       = generatedMesh;
			IsCompleted         = isCompleted;
			MeshSeed            = meshSeed;
			GeneratedRoomMeshes = generatedRoomMeshes;
		}

		[field: SerializeField] public string             MeshSeed            { get; private set; }
		[field: SerializeField] public Mesh               GeneratedMesh       { get; private set; }
		[field: SerializeField] public RoomMeshDictionary GeneratedRoomMeshes { get; private set; }
		[field: SerializeField] public bool               IsCompleted         { get; private set; }
	}
}