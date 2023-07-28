using System;
using System.Collections;
using System.Collections.Generic;
using BCL;
using Sirenix.OdinInspector;
using UnityBCL.Serialization;
#if UNITY_STANDALONE || UNITY_EDITOR
using UnityEditorInternal;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace Procedural {
	[Serializable]
	public class ProceduralPathfindingSolverMonobehaviorModel {
		const string ErosionTagInfo =
			"Be aware: make sure to adjust the traversable tags in your AI's 'Seeker' component. Typically, the tag you want to make 'untraversable' is = Erosion Iterations - 1";

		const string AsyncMsg =
			"If Async is selected, the overall time to process pathfinding will significantly increase, but will not make the application unresponsive.";

		[FormerlySerializedAs("_drawTileTileMapDataLocations")] [SerializeField] [EnumToggleButtons]
		Toggle _drawGizmosIfTileExists = Toggle.No;

		[Title("Gizmos & Debugging")] [SerializeField] [EnumToggleButtons]
		Toggle _drawNodePositionGizmos = Toggle.No;

		[SerializeField] [EnumToggleButtons] Toggle _drawTilePositionGizmos        = Toggle.No;
		[SerializeField] [EnumToggleButtons] Toggle _drawTilePositionShiftedGizmos = Toggle.No;

		[Title("Erosion")] [SerializeField] [EnumToggleButtons]
		Toggle _erodeNodesAtBoundaries = Toggle.Yes;

		[DetailedInfoBox(AsyncMsg, "Async or Sync")] [Title("General")] [SerializeField] [EnumToggleButtons]
		Toggle _runCalculationsAsync = Toggle.Yes;

		[field: SerializeField]
		[field: Required]
		[field: Title("Required Monobehaviors")]
		public GameObject PathfindingGameObject { get; set; }

		[field: SerializeField]
		[field: InlineEditor(InlineEditorObjectFieldModes.Hidden)]
		public SerializerSetup SerializerSetup { get; private set; }

		public bool RunCalculationsAsync          => _runCalculationsAsync          == Toggle.Yes;
		public bool ErodeNodesAtBoundaries        => _erodeNodesAtBoundaries        == Toggle.Yes;
		public bool DrawNodePositionGizmos        => _drawNodePositionGizmos        == Toggle.Yes;
		public bool DrawTilePositionGizmos        => _drawTilePositionGizmos        == Toggle.Yes;
		public bool DrawTilePositionShiftedGizmos => _drawTilePositionShiftedGizmos == Toggle.Yes;
		public bool DrawGizmosIfTileExists        => _drawGizmosIfTileExists        == Toggle.Yes;

		[field: SerializeField]
		[field: Range(.1f, 8)]
		public float GridNodeSize { get; private set; } = 3;

		[Title("Collisions")]
		[field: SerializeField]
		[field: Range(.1f, 8)]
		public float AstarCollisionDiameter { get; private set; } = 1.75f;

		[field: SerializeField]
		[field: ValueDropdown("GetTags")]
		public List<string> ObstacleLayerMasks { get; private set; }

		[field: SerializeField]
		[field: Range(0, 10)]
		[field: ShowIf("@ErodeNodesAtBoundaries")]
		[field: InfoBox(ErosionTagInfo, InfoMessageType.Warning)]
		public int NodesToErodeAtBoundaries { get; private set; } = 3;

		[field: SerializeField]
		[field: Range(0, 18)]
		[field: ShowIf("@ErodeNodesAtBoundaries")]
		public int StartingNodeIndexToErode { get; private set; } = 1;

		[Title("Information")]
		[ShowInInspector]
		[ReadOnly]
		public int NumberOfNodesPerTile { get; set; }

		[ShowInInspector] [ReadOnly] public int Iterator { get; set; }

#if UNITY_EDITOR || UNITY_STANDALONE
		IEnumerable GetTags => InternalEditorUtility.layers;
#endif

		public string SavePrefix => "AstarGraph_";
	}
}