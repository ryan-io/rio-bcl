using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityBCL {
	[Serializable]
	public class MonoSchedulerEvents {
		[SerializeField] public UnityEvent OnUpdate      = null!;
		[SerializeField] public UnityEvent OnFixedUpdate = null!;
		[SerializeField] public UnityEvent OnLateUpdate  = null!;
	}
}