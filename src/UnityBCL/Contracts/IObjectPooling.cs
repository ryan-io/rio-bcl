using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UnityBCL {
	public interface IObjectPooling {
		UniTask CreatePool(PooledObjectSetup pooledObject, int quantity);
		UniTask CreatePool(AssetReference pooledObject, int quantity);
	
		GameObject SpawnObject(PoolJob.SpawnObjectJobSimple jobSpawnSimple);
		
		GameObject SpawnObject(PoolJob.SpawnObjectJobEuler jobSpawnEuler);
		
		GameObject SpawnObject(PoolJob.SpawnObjectJobQuat jobSpawnQuat);

		GameObject SpawnObject(PoolJob.SpawnObjectJobParent jobSpawnParent);

		void DespawnObject(PoolJob.DespawnObjectJob jobDespawn);
	}
}