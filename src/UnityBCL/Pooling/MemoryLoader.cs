using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityBCL {
	public static class MemoryLoader {
		public static void UnloadFromMemory(AsyncOperationHandle<GameObject> operationTask) {
			Addressables.Release(operationTask);
		} 

		public static AsyncOperationHandle<GameObject> LoadIntoMemory(PooledObjectSetup pooledObject) {
			return Addressables.LoadAssetAsync<GameObject>(pooledObject.Asset.RuntimeKey);
		}  
	}
}