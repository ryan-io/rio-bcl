using UnityEngine;

namespace UnityBCL {
	public abstract class SimpleUiFeedback : MonoBehaviour {
		public          bool IsPlaying { get; protected set; }
		public          bool IsStopped { get; protected set; }
		public abstract void StartFeedback();

		public abstract void StopFeedback();
	}
}