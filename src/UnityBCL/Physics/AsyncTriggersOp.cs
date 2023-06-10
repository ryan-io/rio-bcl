using System;
using System.Threading;
using BCL;
using BCL.Unmanaged;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace UnityBCL {
	/// <summary>
	///     Creates an asynchronous wrapper for OnTriggerEnter, OnTriggerStay, & OnTriggerExit. Can register/unregister
	///     outside Actions via ObserveTriggerEnter, ObserveTriggerStay, & ObserveTriggerExit. These Observables are invoked
	///     relative to their respective name. ObserverTriggerStay.Signal is invoked every frame while an antagonist is
	///     present.
	/// </summary>
	public class AsyncTriggersOp {
		[Flags]
		public enum Event {
			OnEnter,
			OnExit,
			OnStay
		}

		readonly Authorizer               _authorizer;
		readonly AsyncColliderOpCallbacks _callbacks;

		readonly CancellationTokenWrapper _cancellation;
		readonly AsyncTriggerEnterTrigger _enterTrigger;
		readonly AsyncTriggerExitTrigger  _exitTrigger;
		readonly ILogging                 _logging;
		readonly AsyncTriggerStayTrigger  _stayTrigger;

		public AsyncTriggersOp(GameObject owner) {
			_authorizer   = new Authorizer(true);
			_enterTrigger = owner.GetAsyncTriggerEnterTrigger();
			_exitTrigger  = owner.GetAsyncTriggerExitTrigger();
			_stayTrigger  = owner.GetAsyncTriggerStayTrigger();
			_callbacks = new AsyncColliderOpCallbacks {
				{ TriggerState.Enter, new AsyncOpCallback<Collider>() },
				{ TriggerState.Exit, new AsyncOpCallback<Collider>() },
				{ TriggerState.Stay, new AsyncOpCallback<Collider>() }
			};
			_logging      = new UnityLogging(this);
			_cancellation = new CancellationTokenWrapper();
		}

		public bool IsStay { get; set; }

		public void SetOnEnterContext(Action<Collider, CancellationToken> ctx)
			=> _callbacks[TriggerState.Enter].Context = ctx;

		public void SetOnExitContext(Action<Collider, CancellationToken> ctx)
			=> _callbacks[TriggerState.Exit].Context = ctx;

		public void SetOnStayContext(Action<Collider, CancellationToken> ctx)
			=> _callbacks[TriggerState.Stay].Context = ctx;

		public void ResetOnEnterContext() => _callbacks[TriggerState.Enter].Context = AsyncOpCallback<Collider>.Empty;

		public void ResetOnExitContext() => _callbacks[TriggerState.Enter].Context = AsyncOpCallback<Collider>.Empty;

		public void ResetOnStayContext() => _callbacks[TriggerState.Stay].Context = AsyncOpCallback<Collider>.Empty;

		public async UniTask RunProcessAsync(Event @event, MonoBehaviour consumer) {
			if (_authorizer.IsAuthorized(Event.OnEnter, @event)) {
				var token = _cancellation.GetTokenWithOnDestroy(consumer);
				await StartListeningOnTriggerEnter(token);
			}
		}

		public void StopProcessAsync() {
			_cancellation.Cancel();
		}

		async UniTask StartListeningOnTriggerEnter(CancellationToken token) {
			var result = await _enterTrigger.OnTriggerEnterAsync(token);
			if (_callbacks[TriggerState.Enter].Context != null)
				_callbacks[TriggerState.Enter].Alert(result, token);
			await StartListeningOnTriggerStay(token);
		}

		async UniTask StartListeningOnTriggerExit(CancellationToken token) {
			var result = await _exitTrigger.OnTriggerExitAsync(token);
			if (_callbacks[TriggerState.Exit].Context != null)
				_callbacks[TriggerState.Exit].Alert(result, token);
			IsStay = false;
		}

		async UniTask StartListeningOnTriggerStay(CancellationToken token) {
			var result = await _stayTrigger.OnTriggerStayAsync(token);

			IsStay = true;
			await UniTask.WhenAll(StartListeningOnTriggerExit(token), OnStay(result, token));
			await StartListeningOnTriggerEnter(token);
		}

		async UniTask OnStay(Collider c, CancellationToken token) {
			while (IsStay) {
				if (token.IsCancellationRequested)
					break;
				await UniTask.Yield();
				if (_callbacks[TriggerState.Stay].Context != null)
					_callbacks[TriggerState.Stay].Alert(c, token);
			}
		}
	}
}