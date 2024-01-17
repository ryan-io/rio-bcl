using System;
using System.Threading;
using RIO.BCL;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityBCL {
	public class AddressableInstantiator {
		readonly ILogging _logging;

		public AddressableInstantiator() => _logging = new UnityLogging(this);

		public void InstantiateAsync(
			AssetReferenceGameObject assetReference, JobNoParent job) {
			var handle = assetReference.InstantiateAsync(job.Position, job.Rotation);

			if (job.Callback != null) handle.Completed += job.Callback.Invoke;
		}

		public void InstantiateAsync(
			AssetReferenceGameObject assetReference, JobParent job) {
			var handle = assetReference.InstantiateAsync(job.Parent);

			if (job.Callback != null) handle.Completed += job.Callback.Invoke;
		}

		public async UniTask<GameObject> InstantiateAsync(AssetReferenceGameObject assetReference,
			CancellationToken token) {
			try {
				var handle = assetReference.InstantiateAsync();

				while (!handle.IsDone) {
					if (token.IsCancellationRequested)
						break;
					await UniTask.Yield();
				}

				return handle.Result;
			}
			catch (Exception) {
				_logging.Log(LogLevel.Warning, "Could not instantiate game object. The handle was invalid.");
				return default!;
			}
		}

		public GameObject Instantiate(AssetReferenceGameObject assetReference)
			=> assetReference.InstantiateAsync().Result;

		public readonly struct JobNoParent {
			public JobNoParent(Vector3 position, Quaternion rotation,
				Action<AsyncOperationHandle<GameObject>> callback) {
				Position = position;
				Rotation = rotation;
				Callback = callback;
			}

			public Vector3                                  Position { get; }
			public Quaternion                               Rotation { get; }
			public Action<AsyncOperationHandle<GameObject>> Callback { get; }
		}

		public readonly struct JobParent {
			public JobParent(Transform parent, Action<AsyncOperationHandle<GameObject>> callback) {
				Parent   = parent;
				Callback = callback;
			}

			public Transform                                Parent   { get; }
			public Action<AsyncOperationHandle<GameObject>> Callback { get; }
		}
	}
}