using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnityBCL {
	public class PoolingProxy : IPoolingProvider {
		readonly IObjectPooling _pooling;

		public PoolingProxy() : this(ObjectPooling.Global) {
		}

		public PoolingProxy(IObjectPooling pooling) => _pooling = pooling;

		public UniTask CreatePool(PooledObjectSetup pooledObject, int quantity)
			=> _pooling.CreatePool(pooledObject, quantity);

		public GameObject SpawnObject(PoolJob.SpawnObjectJobSimple jobSpawnSimple)
			=> _pooling.SpawnObject(jobSpawnSimple);

		public GameObject SpawnObject(PoolJob.SpawnObjectJobEuler jobSpawnEuler)
			=> _pooling.SpawnObject(jobSpawnEuler);

		public GameObject SpawnObject(PoolJob.SpawnObjectJobQuat jobSpawnQuat)
			=> _pooling.SpawnObject(jobSpawnQuat);

		public GameObject SpawnObject(PoolJob.SpawnObjectJobParent jobSpawnParent)
			=> _pooling.SpawnObject(jobSpawnParent);

		public void DespawnObject(PoolJob.DespawnObjectJob jobDespawn)
			=> _pooling.DespawnObject(jobDespawn);
	}
}