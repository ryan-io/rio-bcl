using System;
using UnityEngine;

namespace Procedural {
	[Serializable]
	public struct GeneratorControllerModel {
		[field: SerializeField] public ProceduralMapSolver Generator { get; private set; }
	}
}