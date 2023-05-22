using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UnityBCL {
	[Serializable, InfoBox("Use this container to pool any object at runtime. This is not necessary to use. You can " +
	                        "also add to the pool via ObjectPooling.Global.CreatePool().")]
	public class PoolingMonoModel {
		[field: SerializeField, Title("Pooled Objects")]
		public PooledObjectContainer PooledObjectContainer { get; private set; } = new();

		[field: SerializeField, Title("Debugging")] public bool OutputLogs { get; private set; }
	}
}