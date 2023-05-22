using UnityEngine;

namespace UnityBCL {
	public static class PoolJob {
		public readonly struct SpawnObjectJobSimple {
			public string PoolId      { get; }
			public bool   ActiveState { get; }

			public SpawnObjectJobSimple(string poolId, bool activeState) {
				PoolId      = poolId;
				ActiveState = activeState;
			}
		}
		
		public readonly struct SpawnObjectJobEuler {
			public string  PoolId      { get; }
			public Vector3 Position    { get; }
			public Vector3 Rotation    { get; }
			public bool    ActiveState { get; }

			public SpawnObjectJobEuler(string poolId, Vector3 position, Vector3 rotation, bool activeState) {
				PoolId      = poolId;
				Position    = position;
				Rotation    = rotation;
				ActiveState = activeState;
			}
		}
		
		public readonly struct SpawnObjectJobQuat {
			public string     PoolId      { get; }
			public Vector3    Position    { get; }
			public Quaternion Rotation    { get; }
			public bool       ActiveState { get; }

			public SpawnObjectJobQuat(string poolId, Vector3 position, Quaternion rotation, bool activeState) {
				PoolId      = poolId;
				Position    = position;
				Rotation    = rotation;
				ActiveState = activeState;
			}
		}

		public readonly struct SpawnObjectJobParent {
			public string    PoolId      { get; }
			public Transform Parent      { get; }
			public bool      ActiveState { get; }

			public SpawnObjectJobParent(string poolId, Transform parent, bool activeState) {
				PoolId      = poolId;
				Parent      = parent;
				ActiveState = activeState;
			}
		}

		public readonly struct SpawnObjectJobComplex {
			public string     PoolId      { get; }
			public Transform  Parent      { get; }
			public Vector3    Position    { get; }
			public Quaternion Rotation    { get; }
			public bool       ActiveState { get; }

			public SpawnObjectJobComplex(string poolId, Transform parent, Vector3 position, Quaternion rotation, bool activeState) {
				PoolId           = poolId;
				Parent           = parent;
				Position         = position;
				Rotation         = rotation;
				ActiveState = activeState;
			}
		}

		public readonly struct DespawnObjectJob {
			public string     PoolId         { get; }
			public GameObject ObjectToReturn { get; }

			public DespawnObjectJob(string poolId, GameObject objectToReturn) {
				PoolId         = poolId;
				ObjectToReturn = objectToReturn;
			}
		}
	}
}