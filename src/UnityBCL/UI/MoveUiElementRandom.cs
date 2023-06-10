using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

namespace UnityBCL {
	public class MoveUiElementRandom : MonoBehaviour {
		[SerializeField] [BoxGroup("0", false)]
		RectTransform _rectTransform = null!;

		[SerializeField] [Range(10, 100f)] [BoxGroup("0", false)]
		float _lengthMultiplier = 25f;

		[SerializeField] [Range(0.05f, 50f)] [BoxGroup("0", false)]
		float _linearDuration = .35f;

		CancellationTokenSource _cancellation = null!;
		Vector2                 _lerpFromVector;
		Vector2                 _lerpToVector;
		Vector2                 _origin;
		Random                  _sysRandom = null!;

		void Awake() {
			_cancellation = new CancellationTokenSource();
			_sysRandom    = new Random();
			_origin       = _rectTransform.anchoredPosition;
		}

		void Start()     => StartLerp();
		void OnEnable()  => _cancellation = new CancellationTokenSource();
		void OnDisable() => StopTask(false);

		async void StartLerp() {
			if (!_rectTransform)
				return;
#if UNITY_EDITOR || UNITY_STANDALONE
			//Logger.Msg("Starting lerp");
#endif
			SetLerpVector();

			await LerpRect();
		}

		IEnumerator LerpRect() {
			var time = 0f;

			while (true) {
				if (_cancellation.Token.IsCancellationRequested) {
#if UNITY_EDITOR || UNITY_STANDALONE
					//Logger.Warning("Cancelling MovUiElementRandom");
#endif
					yield break;
				}

				var update = time / _linearDuration;
				_rectTransform.anchoredPosition =  Vector2.Lerp(_lerpFromVector, _lerpToVector, update);
				time                            += Time.deltaTime;

				if (time > _linearDuration) {
					time = 0;
					SetLerpVector();
				}

				yield return null;
			}
		}

		void SetLerpVector() {
			var chance = _sysRandom.NextBool();
			var randomPoint = chance
				                  ? _lengthMultiplier * UnityEngine.Random.insideUnitCircle
				                  : -1                * _lengthMultiplier * UnityEngine.Random.insideUnitCircle;

			var additiveVector   = new Vector2(_origin.x + randomPoint.x, _origin.y + randomPoint.y);
			var anchoredPosition = _rectTransform.anchoredPosition;

			_lerpFromVector = anchoredPosition;
			_lerpToVector   = _origin + additiveVector;
		}

		void StopTask(bool createNew = true) {
			_cancellation?.Cancel();
			_cancellation?.Dispose();

			if (createNew)
				_cancellation = new CancellationTokenSource();
		}
	}
}