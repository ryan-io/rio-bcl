using System;

namespace UnityBCL {
	[Serializable]
	public class PooledObjectContainer : SerializedDictionary<int, PooledObjectSetup> {
	}
}