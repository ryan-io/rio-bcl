using System;
using UnityEngine;
#if UNITY_EDITOR || UNITY_STANDALONE
using Sirenix.OdinInspector;
#endif

namespace UnityBCL {
	[Serializable]
	[InfoBox("Use this container to pool any object at runtime. This is not necessary to use. You can " +
	         "also add to the pool via ObjectPooling.Global.CreatePool().")]
	public class PoolingMonoModel {
		[field: SerializeField]
		[field: Title("Pooled Objects")]
		public PooledObjectContainer PooledObjectContainer { get; private set; } = new();

		[field: SerializeField]
		[field: Title("Debugging")]
		public bool OutputLogs { get; private set; }
	}
}