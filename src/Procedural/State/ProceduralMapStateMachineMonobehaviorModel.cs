// Engine.Procedural

using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Procedural {
	[Serializable]
	public class ProceduralMapStateMachineMonobehaviorModel {
		[field: SerializeField]
		[field: Title("Required Monobehaviors")]
		[field: Required]
		public ProceduralController ProceduralController { get; set; }
	}
}