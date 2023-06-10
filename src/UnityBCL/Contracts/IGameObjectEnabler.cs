using UnityEngine;

namespace UnityBCL {
	public interface IGameObjectEnabler : IGameObjectEnablerList {
		void Add(GameObject obj);
		void Remove(GameObject obj);
	}
}