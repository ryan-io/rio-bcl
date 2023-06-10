using System;
using UnityEngine.Events;

namespace Procedural {
	[Serializable]
	public class OnCompleteWithDataEvent : UnityEvent<MapGenerationData> {
	}
}