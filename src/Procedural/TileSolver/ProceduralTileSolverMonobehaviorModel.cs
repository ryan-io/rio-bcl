using System;
using BCL;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Procedural {
	[Serializable]
	public class ProceduralTileSolverMonobehaviorModel {
		public Tilemap BoundaryTilemap => TileMapGameObjects.TileMapTable[TileMapType.Boundary];
		public Tilemap GroundTilemap   => TileMapGameObjects.TileMapTable[TileMapType.Ground];

#region Procedural Configurations			---------------------------------------->

		[field: SerializeField]
		[field: Title("Tile Solver Configuration")]
		[field: Required]
		public ProceduralTilePlacementConfiguration ProceduralTilePlacementConfig { get; private set; }

#endregion

		[Button(ButtonSizes.Medium)]
		[GUIColor(255 / 255f, 41 / 255f, 84 / 255f)]
		[ShowIf("@ProceduralTilePlacementConfig != null")]
		void SetIniToNull() => ProceduralTilePlacementConfig = null;

#region General Configuration			---------------------------------------->

		[field: SerializeField]
		[field: EnumToggleButtons]
		[field: Title("General")]
		public Toggle GenerateTiles { get; private set; } = Toggle.Yes;

		[field: SerializeField]
		[field: EnumToggleButtons]
		public TileProcessMethod TileProcessMethod { get; private set; } = TileProcessMethod.Async;

#endregion

#region Serialized Required Monobehaviors	---------------------------------------->

		[field: SerializeField]
		[field: Title("Required Monobehaviors")]
		[field: Required]
		public ProceduralMapSolver ProceduralMapSolver { get; set; }

		[field: SerializeField]
		[field: Required]
		[field: HideLabel]
		public ProceduralTileSceneObjects TileMapGameObjects { get; set; }

#endregion
	}
}