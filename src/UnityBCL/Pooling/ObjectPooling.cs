using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using BCL;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace UnityBCL {
	public class ObjectPooling : Singleton<ObjectPooling, IObjectPooling>, IObjectPooling {
		static readonly Vector3 PoolLocation = new(10000f, 10000f, 0);

		[FormerlySerializedAs("_model")] [SerializeField] [HideLabel]
		public PoolingMonoModel Model = new();

		[field: SerializeField, Title("On Initialization Complete Callback")]
		public ScriptableUnityEvent[] OnInitComplete { get; private set; }

		CancellationTokenSource _cancellationTokenSource = null!;
		PoolQueue               _poolQueue               = null!;
		[ShowInInspector] int   QueueCount => _poolQueue.QueueCount;

		void Awake() {
			_poolQueue               = new PoolQueue();
			_cancellationTokenSource = new CancellationTokenSource();

			var unitOfWork = new EmptyUnitOfWorkCtx(
				async _ => {
					var token = gameObject.GetCancellationTokenOnDestroy();

					foreach (var pooledObject in Model.PooledObjectContainer) {
						if (pooledObject.Value == null)
							continue;

						if (token.IsCancellationRequested) {
							throw new OperationCanceledException("Application shutdown. Pooling not complete.");
						}

						await CreatePool(pooledObject.Value, pooledObject.Key);
					}

					foreach (var t in OnInitComplete)
						if (t)
							t.InvokeEvent();
				});
			
			unitOfWork.FireForget(this.GetCancellationTokenOnDestroy());
		}

		void OnDisable() {
			if (!_cancellationTokenSource.IsCancellationRequested)
				_cancellationTokenSource?.Cancel();
			_cancellationTokenSource?.Dispose();
		}

		public async UniTask CreatePool(PooledObjectSetup pooledObject, int quantity) {
			if (PoolQueue.CannotPool(pooledObject, quantity)) {
				if (!Application.isEditor)
					return;

				var log = new UnityLogging(this);
				log.Log(LogLevel.Warning, $"Cannot pool {pooledObject.name}");

				return;
			}

			// non-determinstic order of placement within the queue
			var queue = new ConcurrentQueue<GameObject>();
			var bag   = new ConcurrentBag<UniTask<GameObject>>();

			for (var i = 0; i < quantity; i++) {
				var task = pooledObject.Asset.InstantiateAsync();
				task.Completed += handle => {
					                  handle.Result.transform.position = PoolLocation;
					                  handle.Result.SetActive(false);
					                  queue.Enqueue(handle.Result);
					                  if (Model.OutputLogs && Application.isEditor) {
						                  var log = new UnityLogging(this);
						                  log.Log(LogLevel.Warning,
							                  "Object Pooling -  Adding " + handle.Result.name + " to the pool!");
					                  }
				                  };
				bag.Add(task.ToUniTask());
			}

			await UniTask.WhenAll(bag);
			var queueNonConcurrent = new Queue<GameObject>(queue);
			_poolQueue.AddToQueueDictionary(pooledObject, queueNonConcurrent);
		}

		public async UniTask CreatePool(AssetReference assetReference, int quantity) {
			var objectSetup = ScriptableObject.CreateInstance<PooledObjectSetup>();
			objectSetup.Asset = assetReference;
			await CreatePool(objectSetup, quantity);
		}

		public GameObject SpawnObject(PoolJob.SpawnObjectJobSimple jobSpawnSimple) {
			var queueObject = _poolQueue.Dequeue(jobSpawnSimple.PoolId, Model.OutputLogs);

			if (queueObject == null) {
				if (!Application.isEditor) {
					var log = new UnityLogging(this);
					log.Log(LogLevel.Warning, "Could not find a queue object.");
				}

				return default!;
			}

			PoolingTransformProcessor.SetPooledObjectActiveState(queueObject, jobSpawnSimple.ActiveState);

			return queueObject;
		}

		public GameObject SpawnObject(PoolJob.SpawnObjectJobQuat jobSpawnQuat) {
			var queueObject = _poolQueue.Dequeue(jobSpawnQuat.PoolId, Model.OutputLogs);

			if (queueObject == null) {
				if (!Application.isEditor) {
					var log = new UnityLogging(this);
					log.Log(LogLevel.Warning, "Could not find a queue object.");
				}

				return default!;
			}

			PoolingTransformProcessor.SetPositionAndRotation(jobSpawnQuat, queueObject);
			PoolingTransformProcessor.SetPooledObjectActiveState(queueObject, jobSpawnQuat.ActiveState);

			return queueObject;
		}

		public GameObject SpawnObject(PoolJob.SpawnObjectJobEuler jobSpawnEuler) {
			var queueObject = _poolQueue.Dequeue(jobSpawnEuler.PoolId, Model.OutputLogs);

			if (queueObject == null) {
				if (!Application.isEditor) {
					var log = new UnityLogging(this);
					log.Log(LogLevel.Warning, "Could not find a queue object.");
				}

				return default!;
			}

			PoolingTransformProcessor.SetPositionAndRotation(jobSpawnEuler, queueObject);
			PoolingTransformProcessor.SetPooledObjectActiveState(queueObject, jobSpawnEuler.ActiveState);

			return queueObject;
		}

		public GameObject SpawnObject(PoolJob.SpawnObjectJobParent jobSpawnParent) {
			var queueObject = _poolQueue.Dequeue(jobSpawnParent.PoolId, Model.OutputLogs);

			if (queueObject == null)
				return default!;

			PoolingTransformProcessor.SetPositionAndRotation(jobSpawnParent, queueObject);
			PoolingTransformProcessor.SetPooledObjectActiveState(queueObject, jobSpawnParent.ActiveState);

			return queueObject;
		}

		public void DespawnObject(PoolJob.DespawnObjectJob jobDespawn) {
			PoolingTransformProcessor.ResetPositionAndRotation(jobDespawn.ObjectToReturn);
			PoolingTransformProcessor.DeactivatePooledObject(jobDespawn.ObjectToReturn);
			jobDespawn.ObjectToReturn.transform.position = PoolLocation;

			_poolQueue.Enqueue(jobDespawn.PoolId, jobDespawn.ObjectToReturn);
		}
	}
}