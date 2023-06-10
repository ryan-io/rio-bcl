using UnityEngine;

namespace Procedural {
	public abstract class PoissonMod : ScriptableObject {
		public abstract void Process(Transform tr);
	}
}