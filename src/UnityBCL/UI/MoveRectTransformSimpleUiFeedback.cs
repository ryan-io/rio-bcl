using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UnityBCL {
	public class MoveRectTransformSimpleUiFeedback : SimpleUiFeedback {
		[SerializeField] RectTransform _rectTransform = null!;
		[SerializeField] bool          _lerpPositive  = true;

		// [SerializeField, ShowIf("@_calculateNewVectorEachTime")]
		// bool _allowRandomPosNeg = false;

		[SerializeField]                      bool  _calculateNewVectorEachTime;
		[SerializeField] [Range(0.0f,  100f)] float _xPosOffset;
		[SerializeField] [Range(0.0f,  100f)] float _xNegOffset;
		[SerializeField] [Range(0.0f,  100f)] float _yPosOffset;
		[SerializeField] [Range(0.0f,  100f)] float _yNegOffset;
		[SerializeField] [Range(0.05f, 50f)]  float _linearDuration = .35f;

		CancellationTokenSource? _cancellationStart;
		CancellationTokenSource? _cancellationStop;
		bool                     _hasFinished;
		Vector2                  _lerpVector;
		Vector2                  _origin;

		void Start() {
			if (_rectTransform == null)
				return;

			_origin            = _rectTransform.anchoredPosition;
			_cancellationStart = new CancellationTokenSource();
			_cancellationStop  = new CancellationTokenSource();
			SetLerpVector();
		}

		void OnDisable() {
			_cancellationStart?.Cancel();
			_cancellationStop?.Cancel();
			_cancellationStart?.Dispose();
			_cancellationStop?.Dispose();

			_cancellationStart = _cancellationStop = null;
		}

		void OnDestroy() => OnDisable();

		public override async void StartFeedback() {
			if (!_rectTransform)
				return;

			IsPlaying = true;
			IsStopped = false;

			_cancellationStart = new CancellationTokenSource();
			_cancellationStop?.Cancel();
			_cancellationStop?.Dispose();

			if (_calculateNewVectorEachTime)
				SetLerpVector();

			var enumerator = _hasFinished
				                 ? LerpRect(_origin, _lerpVector, _linearDuration, _cancellationStart.Token)
				                 : LerpRect(_rectTransform.anchoredPosition, _lerpVector, _linearDuration,
					                 _cancellationStart.Token);

			await enumerator;

			_cancellationStart = null;
		}

		public override async void StopFeedback() {
			if (!_rectTransform)
				return;

			IsPlaying = false;
			IsStopped = true;

			_cancellationStop = new CancellationTokenSource();
			_cancellationStart?.Cancel();
			_cancellationStart?.Dispose();

			var enumerator = _hasFinished
				                 ? LerpRect(_lerpVector, _origin, _linearDuration, _cancellationStop.Token)
				                 : LerpRect(_rectTransform.anchoredPosition, _origin, _linearDuration,
					                 _cancellationStop.Token);

			await enumerator;

			_cancellationStop = null;
		}

		public void LogTest() {
			//Debug.Log("RectTrMethodInvoked via events");
		}

		void SetLerpVector() {
			_lerpVector = _lerpPositive
				              ? new Vector2(_origin.x + _xPosOffset, _origin.y + _yPosOffset)
				              : new Vector2(_origin.x - _xNegOffset, _origin.y - _yNegOffset);
		}

		IEnumerator LerpRect(Vector2 lerpFrom, Vector2 lerpTo, float duration, CancellationToken token) {
			var time = 0f;
			_hasFinished = false;

			while (Vector2.Distance(lerpFrom, lerpTo) >= 0.005f) {
				if (token.IsCancellationRequested)
					yield break;

				var update = time / duration;
				_rectTransform.anchoredPosition =  Vector2.Lerp(lerpFrom, lerpTo, update);
				time                            += Time.deltaTime;

				yield return null;
			}

			_rectTransform.anchoredPosition = lerpTo;
			IsPlaying                       = false;
			_hasFinished                    = true;
		}

#if UNITY_EDITOR || UNITY_STANDALONE

		[Button]
		void InvokeSetLerpVector() {
			if (!_rectTransform)
				return;

			StartFeedback();
		}

		[Button]
		void StopLerping() {
			StopFeedback();
		}

#endif
	}
}