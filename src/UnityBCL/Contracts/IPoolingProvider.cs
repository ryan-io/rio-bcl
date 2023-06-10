using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnityBCL {
	public interface IPoolingProvider {
		UniTask CreatePool(PooledObjectSetup pooledObject, int quantity);

		GameObject SpawnObject(PoolJob.SpawnObjectJobSimple jobSpawnSimple);

		GameObject SpawnObject(PoolJob.SpawnObjectJobEuler jobSpawnEuler);

		GameObject SpawnObject(PoolJob.SpawnObjectJobQuat jobSpawnQuat);

		GameObject SpawnObject(PoolJob.SpawnObjectJobParent jobSpawnParent);

		void DespawnObject(PoolJob.DespawnObjectJob jobDespawn);
	}
}