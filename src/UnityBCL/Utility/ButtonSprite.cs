using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityBCL {
	public class ButtonSprite : MonoBehaviour {
		static Func<Camera>? _getWorldCamera;

		public static void SetGetWorldCamera(Func<Camera> worldCamera) {
			_getWorldCamera = worldCamera;
		}

		public Action? ClickFunc                = null;
		public Action? MouseRightDownOnceFunc   = null;
		public Action? MouseRightDownFunc       = null;
		public Action? MouseRightUpFunc         = null;
		public Action? MouseDownOnceFunc        = null;
		public Action? MouseUpOnceFunc          = null;
		public Action? MouseOverOnceFunc        = null;
		public Action? MouseOutOnceFunc         = null;
		public Action? MouseOverOnceTooltipFunc = null;
		public Action? MouseOutOnceTooltipFunc  = null;

		bool                                      _draggingMouseRight;
		Vector3                                   _mouseRightDragStart;
		public readonly Action<Vector3, Vector3>? MouseRightDragFunc       = null;
		public readonly Action<Vector3, Vector3>? MouseRightDragUpdateFunc = null;
		public          bool                      triggerMouseRightDragOnEnter;

		public enum HoverBehaviour {
			Custom,
			ChangeColor,
			ChangeImage,
			ChangeSetActive
		}

		public HoverBehaviour hoverBehaviourType       = HoverBehaviour.Custom;
		Action                _hoverBehaviourFuncEnter = null!, _hoverBehaviourFuncExit = null!;

		public Color hoverBehaviour_Color_Enter = new(1, 1, 1, 1),
		             hoverBehaviour_Color_Exit  = new(1, 1, 1, 1);

		public SpriteRenderer hoverBehaviour_Image       = null!;
		public Sprite         hoverBehaviour_Sprite_Exit = null!, hoverBehaviour_Sprite_Enter = null!;
		public bool           hoverBehaviour_Move;
		public Vector2        hoverBehaviour_Move_Amount = Vector2.zero;
		Vector3               _posExit, _posEnter;
		public bool           triggerMouseOutFuncOnClick;
		public bool           clickThroughUI;

		readonly Action _internalOnMouseDownFunc  = null!;
		readonly Action _internalOnMouseEnterFunc = null!;
		readonly Action _internalOnMouseExitFunc  = null!;

#if SOUND_MANAGER
        public Sound_Manager.Sound mouseOverSound, mouseClickSound;
#endif
#if CURSOR_MANAGER
        public CursorManager.CursorType cursorMouseOver, cursorMouseOut;
#endif


		public void SetHoverBehaviourChangeColor(Color colorOver, Color colorOut) {
			hoverBehaviourType         = HoverBehaviour.ChangeColor;
			hoverBehaviour_Color_Enter = colorOver;
			hoverBehaviour_Color_Exit  = colorOut;
			if (hoverBehaviour_Image == null) hoverBehaviour_Image = transform.GetComponent<SpriteRenderer>();
			hoverBehaviour_Image.color = hoverBehaviour_Color_Exit;
			SetupHoverBehaviour();
		}

		void OnMouseDown() {
			if (!clickThroughUI && IsPointerOverUI()) return; // Over UI!

			if (_internalOnMouseDownFunc != null) _internalOnMouseDownFunc();
			if (ClickFunc                != null) ClickFunc();
			if (MouseDownOnceFunc        != null) MouseDownOnceFunc();
			if (triggerMouseOutFuncOnClick) OnMouseExit();
		}

		public void Manual_OnMouseExit() {
			OnMouseExit();
		}

		void OnMouseUp() {
			if (MouseUpOnceFunc != null) MouseUpOnceFunc();
		}

		void OnMouseEnter() {
			if (!clickThroughUI && IsPointerOverUI()) return; // Over UI!

			if (_internalOnMouseEnterFunc != null) _internalOnMouseEnterFunc();
			if (hoverBehaviour_Move) transform.localPosition = _posEnter;
			if (_hoverBehaviourFuncEnter != null) _hoverBehaviourFuncEnter();
			if (MouseOverOnceFunc        != null) MouseOverOnceFunc();
			if (MouseOverOnceTooltipFunc != null) MouseOverOnceTooltipFunc();
		}

		void OnMouseExit() {
			if (_internalOnMouseExitFunc != null) _internalOnMouseExitFunc();
			if (hoverBehaviour_Move) transform.localPosition = _posExit;
			if (_hoverBehaviourFuncExit != null) _hoverBehaviourFuncExit();
			if (MouseOutOnceFunc        != null) MouseOutOnceFunc();
			if (MouseOutOnceTooltipFunc != null) MouseOutOnceTooltipFunc();
		}

		void OnMouseOver() {
			if (!clickThroughUI && IsPointerOverUI()) return; // Over UI!

			if (Input.GetMouseButton(1)) {
				if (MouseRightDownFunc != null) MouseRightDownFunc();
				if (!_draggingMouseRight && triggerMouseRightDragOnEnter) {
					_draggingMouseRight  = true;
					_mouseRightDragStart = GetWorldPositionFromUI();
				}
			}

			if (Input.GetMouseButtonDown(1)) {
				_draggingMouseRight  = true;
				_mouseRightDragStart = GetWorldPositionFromUI();
				if (MouseRightDownOnceFunc != null) MouseRightDownOnceFunc();
			}
		}

		void Update() {
			if (_draggingMouseRight)
				if (MouseRightDragUpdateFunc != null)
					MouseRightDragUpdateFunc(_mouseRightDragStart, GetWorldPositionFromUI());
			if (Input.GetMouseButtonUp(1)) {
				if (_draggingMouseRight) {
					_draggingMouseRight = false;
					if (MouseRightDragFunc != null) MouseRightDragFunc(_mouseRightDragStart, GetWorldPositionFromUI());
				}

				if (MouseRightUpFunc != null) MouseRightUpFunc();
			}
		}


		void Awake() {
			if (_getWorldCamera == null) SetGetWorldCamera(() => Camera.main); // Set default World Camera
			_posExit  = transform.localPosition;
			_posEnter = transform.localPosition + (Vector3)hoverBehaviour_Move_Amount;
			SetupHoverBehaviour();

#if SOUND_MANAGER
            // Sound Manager
            internalOnMouseDownFunc +=
 () => { if (mouseClickSound != Sound_Manager.Sound.None) Sound_Manager.PlaySound(mouseClickSound); };
            internalOnMouseEnterFunc +=
 () => { if (mouseOverSound != Sound_Manager.Sound.None) Sound_Manager.PlaySound(mouseOverSound); };
#endif

#if CURSOR_MANAGER
            // Cursor Manager
            internalOnMouseExitFunc +=
 () => { if (cursorMouseOut != CursorManager.CursorType.None) CursorManager.SetCursor(cursorMouseOut); };
            internalOnMouseEnterFunc +=
 () => { if (cursorMouseOver != CursorManager.CursorType.None) CursorManager.SetCursor(cursorMouseOver); };
#endif
		}

		void SetupHoverBehaviour() {
			switch (hoverBehaviourType) {
				case HoverBehaviour.ChangeColor:
					_hoverBehaviourFuncEnter = delegate { hoverBehaviour_Image.color = hoverBehaviour_Color_Enter; };
					_hoverBehaviourFuncExit  = delegate { hoverBehaviour_Image.color = hoverBehaviour_Color_Exit; };
					break;
				case HoverBehaviour.ChangeImage:
					_hoverBehaviourFuncEnter = delegate { hoverBehaviour_Image.sprite = hoverBehaviour_Sprite_Enter; };
					_hoverBehaviourFuncExit  = delegate { hoverBehaviour_Image.sprite = hoverBehaviour_Sprite_Exit; };
					break;
				case HoverBehaviour.ChangeSetActive:
					_hoverBehaviourFuncEnter = delegate { hoverBehaviour_Image.gameObject.SetActive(true); };
					_hoverBehaviourFuncExit  = delegate { hoverBehaviour_Image.gameObject.SetActive(false); };
					break;
			}
		}


		static Vector3 GetWorldPositionFromUI() {
			if (_getWorldCamera != null) {
				var worldPosition = _getWorldCamera().ScreenToWorldPoint(Input.mousePosition);
				return worldPosition;
			}

			return Vector3.zero;
		}

		static bool IsPointerOverUI() {
			if (EventSystem.current.IsPointerOverGameObject()) return true;

			var pe = new PointerEventData(EventSystem.current);
			pe.position = Input.mousePosition;
			var hits = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pe, hits);
			return hits.Count > 0;
		}
	}
}