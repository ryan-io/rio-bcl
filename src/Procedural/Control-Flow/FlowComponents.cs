using System;
using Sirenix.OdinInspector;
using UnityBCL.Serialization;
using UnityBCL.Serialization.Core;
using UnityEngine;

namespace Procedural {
	[Serializable]
	public class FlowComponents {
		[field: SerializeField]
		[field: Title("Required Components")]
		public ProceduralMapSolver ProceduralMapSolver { get; set; }

		[field: SerializeField]
		[field: Required]
		public ProceduralMeshSolver ProceduralMeshSolver { get; set; }

		[field: SerializeField]
		[field: Required]
		public ProceduralTileSolver ProceduralTileSolver { get; set; }

		[field: SerializeField]
		[field: Required]
		public ProceduralPathfindingSolver ProceduralPathfindingSolver { get; set; }

		[field: SerializeField]
		[field: Required]
		public ProceduralMapStateMachine ProceduralMapStateMachine { get; set; }

		[field: SerializeField]
		[field: Title("Events")]
		[field: Required]
		public ProceduralGenerationEvents Events { get; set; }

		[field: SerializeField]
		[field: Title("Other Dependencies")]
		[field: Required]
		public ProceduralUtility ProceduralUtilityCreation { get; set; }

		[field: SerializeField]
		[field: Required]
		public ProceduralScaler ProceduralScaler { get; set; }

		[field: SerializeField]
		[field: Required]
		public ProceduralController ProceduralController { get; set; }

		[field: SerializeField]
		[field: Required]
		public SerializerSetup SerializerSetup { get; set; }
	}
}