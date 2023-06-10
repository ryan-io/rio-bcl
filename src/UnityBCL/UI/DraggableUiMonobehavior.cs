using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityBCL {
	public class DraggableUiMonobehavior : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler,
	                                       IEndDragHandler, IDragHandler {
		[SerializeField] Canvas _parentCanvas = null!;
		[SerializeField] bool   _resetPositionOnEndDrag;
		CanvasGroup?            _canvasGroup;
		Vector2                 _onClickPosition;

		RectTransform? _rectTransform;

		void Awake() {
			SetRectTransform();
			SetCanvasGroup();
		}

		void Reset() {
			if (_canvasGroup != null) {
				_canvasGroup.blocksRaycasts = true;
				_canvasGroup.alpha          = 1.0f;
			}
		}

		public void OnBeginDrag(PointerEventData eventData) => Modify();

		public void OnDrag(PointerEventData eventData) {
			if (_rectTransform != null) _rectTransform.anchoredPosition += eventData.delta / _parentCanvas.scaleFactor;
		}

		public void OnEndDrag(PointerEventData eventData) {
			if (!IsHoverOnDroppableUI(eventData) && _resetPositionOnEndDrag) ResetToLastOnClickPosition();
			Reset();
		}

		public void OnPointerDown(PointerEventData eventData) {
			if (_rectTransform != null) _onClickPosition = _rectTransform.position;
		}

		public void OnPointerUp(PointerEventData eventData) {
		}

		void Modify() {
			if (_canvasGroup != null) {
				_canvasGroup.blocksRaycasts = false;
				_canvasGroup.alpha          = 0.35f;
			}
		}

		void SetCanvasGroup() {
			_canvasGroup = GetComponent<CanvasGroup>();

			if (_canvasGroup == null)
				_canvasGroup = gameObject.AddComponent<CanvasGroup>();
		}

		void SetRectTransform() {
			_rectTransform = GetComponent<RectTransform>();

			if (_rectTransform == null)
				_rectTransform = gameObject.AddComponent<RectTransform>();
		}

		void ResetToLastOnClickPosition() {
			if (_rectTransform != null) _rectTransform.position = _onClickPosition;
		}

		bool IsHoverOnDroppableUI(PointerEventData eventData) => eventData.pointerCurrentRaycast.gameObject != null
		                                                         && eventData.pointerCurrentRaycast.gameObject
		                                                                     .GetComponent<IDropHandler>() != null;
	}
}