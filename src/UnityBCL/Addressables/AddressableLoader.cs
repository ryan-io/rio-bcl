using System;
using BCL;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityBCL {
	public class AddressableLoader {
		public AsyncOperationHandle<T> LoadReference<T>(AssetReference reference,
			Action<AsyncOperationHandle<T>> callback = null) {
			if (reference.IsValid()) {
				return default;
			}

			var operationHandle = reference.LoadAssetAsync<T>();
			operationHandle.Completed += handle => {
				                             if (handle.Status == AsyncOperationStatus.Succeeded) {
					                             callback?.Invoke(handle);
				                             }
			                             };

			return operationHandle;
		}

		public void ReleaseHandle(AsyncOperationHandle handle) {
			UnityEngine.AddressableAssets.Addressables.Release(handle);
		}

		public void ReleaseAssetReference(AssetReference assetReference) {
			UnityEngine.AddressableAssets.Addressables.Release(assetReference);
		}

		public bool ReleaseAssetInstance(GameObject gameObject) {
			return UnityEngine.AddressableAssets.Addressables.ReleaseInstance(gameObject);
		}

		public AddressableLoader() {
			_logging = new UnityLogging(this, isEnabled: false);
		}

		public AddressableLoader(ILogging logging) {
			_logging = logging;
		}

		readonly ILogging _logging;
	}
}