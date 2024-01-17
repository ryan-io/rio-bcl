using System;
using RIO.BCL;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityBCL {
	public class AddressableLoader {
		readonly ILogging _logging;

		public AddressableLoader() => _logging = new UnityLogging(this, isEnabled: false);

		public AddressableLoader(ILogging logging) => _logging = logging;

		public AsyncOperationHandle<T> LoadReference<T>(AssetReference reference,
			Action<AsyncOperationHandle<T>> callback = null!) {
			if (reference.IsValid()) return default;

			var operationHandle = reference.LoadAssetAsync<T>();
			operationHandle.Completed += handle => {
				                             if (handle.Status == AsyncOperationStatus.Succeeded)
					                             callback?.Invoke(handle);
			                             };

			return operationHandle;
		}

		public void ReleaseHandle(AsyncOperationHandle handle) {
			Addressables.Release(handle);
		}

		public void ReleaseAssetReference(AssetReference assetReference) {
			Addressables.Release(assetReference);
		}

		public bool ReleaseAssetInstance(GameObject gameObject) => Addressables.ReleaseInstance(gameObject);
	}
}