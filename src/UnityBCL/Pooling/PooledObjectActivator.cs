using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityBCL {
	public class ObjectActivator {
		readonly Predicate<bool>? _criteria;

		readonly GameObject _objectToDisable;

		public ObjectActivator(GameObject objectToDisable) {
			_objectToDisable = objectToDisable;
			_criteria        = null;
		}

		public ObjectActivator(GameObject objectToDisable, Predicate<bool> criteria) {
			_objectToDisable = objectToDisable;
			_criteria        = criteria;
		}

		public void Despawn() {
			var hasPoolInfo = _objectToDisable.TryGetComponent(out IPooledObject poolInfo);

			if (hasPoolInfo)
				DespawnWithPoolInfo(poolInfo, _objectToDisable);
			else
				Destroy();
		}

		void DespawnWithPoolInfo(IPooledObject poolInfo, GameObject gameObject) {
			var              despawnJob = new PoolJob.DespawnObjectJob(poolInfo.Identifier, gameObject);
			IPoolingProvider pooling    = new PoolingProxy();
			pooling.DespawnObject(despawnJob);
		}

		public void Destroy() {
			Destroy(_objectToDisable);
		}

		public void Destroy(GameObject gameObject) {
			if (!gameObject)
				return;

			if (Application.isPlaying)
				Object.Destroy(gameObject);
			else
				Object.DestroyImmediate(gameObject);
		}

		public void ForceEnable()  => _objectToDisable.SetActive(true);
		public void ForceDisable() => _objectToDisable.SetActive(false);

		public async UniTaskVoid EnableAfter(float duration, CancellationToken token) {
			if (_criteria != null)
				throw new Exception("Please use the secondary signature when disabling an object that " +
				                    "has criteria to check before disabling.");
			await ChangeObjectState(duration, true, true, token);
		}

		public async UniTaskVoid EnableAfter(float duration, bool withCriteria, CancellationToken token) {
			await ChangeObjectState(duration, withCriteria, true, token);
		}

		public async UniTaskVoid DisableAfter(float duration, CancellationToken token) {
			// if (_criteria != null)
			// 	throw new Exception("Please use the secondary signature when disabling an object that " +
			// 	                    "has criteria to check before disabling.");
			await ChangeObjectState(duration, true, false, token);
		}

		public async UniTaskVoid DisableAfter(float duration, bool withCriteria, CancellationToken token) {
			await ChangeObjectState(duration, withCriteria, false, token);
		}

		async UniTask ChangeObjectState(float duration, bool withCriteria, bool state, CancellationToken token) {
			var unitOfWork = new PooledObjectStateChangeUnitOfWork();
			var args = new Arguments.PooledObjectStateChangeArgs(duration, withCriteria, state, token, _objectToDisable,
				_criteria);
			await unitOfWork.FireTask(args, token);
		}
	}
}