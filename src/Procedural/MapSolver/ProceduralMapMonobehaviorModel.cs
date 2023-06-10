using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Procedural {
	[Serializable]
	public class ProceduralMapMonobehaviorModel {
// #region Serialized Required Monobehaviors	---------------------------------------->
//
// 		[field: SerializeField, Title("Required Monobehaviors")] 
// 		public GameObject PathfindingGameObject { get; private set; }
//
// #endregion

#region Procedural Configurations			---------------------------------------->

		[field: SerializeField]
		[field: Title("Map Solver Configuration")]
		[field: Required]
		public ProceduralMapConfiguration ProceduralProceduralMapConfig { get; private set; }

		[Button(ButtonSizes.Medium)]
		[GUIColor(255 / 255f, 41 / 255f, 84 / 255f)]
		[ButtonGroup("Null")]
		[ShowIf("@ProceduralProceduralMapConfig != null")]
		void SetMapConfigNull() => ProceduralProceduralMapConfig = null;

#endregion

#region Generated & Serialized Room Data	---------------------------------------->

		[field: SerializeField]
		[field: Title("Room Data - Generated")]
		[field: ReadOnly]
		public RoomData RoomDataGenerated { get; set; }

		[field: SerializeField]
		[field: Title("Room Data - Serialized")]
		[field: HideLabel]
		[field: ReadOnly]
		public RoomData RoomDataPreGenerated { get; set; }

#endregion
	}
}