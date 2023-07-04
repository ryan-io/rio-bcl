using UnityEngine;

namespace UnityBCL {
	public abstract class ScriptableUnityEvent : ScriptableObject, IScriptableUnityEvent {
		public abstract void InvokeEvent();
	}

	public interface IScriptableUnityEvent {
		void InvokeEvent();
	}
}