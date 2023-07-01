using UnityEngine;

namespace ProceduralAuxiliary.PoissonSpawning {
	public abstract class PoissonMod : ScriptableObject {
		public abstract void Process(Transform tr);
	}
}