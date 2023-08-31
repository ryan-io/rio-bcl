using UnityEngine;

// by Pepijn Willekens
// https://twitter.com/PepijnWillekens
// pepijnwillekens@gmail.com
namespace ProceduralAuxiliary.ProceduralCollider {
	public static class MonoBehaviourExtension {
		public static T AddMissingComponent<T>(this GameObject gameObject) where T : Component {
			var comp               = gameObject.GetComponent<T>();
			if (comp == null) comp = gameObject.AddComponent<T>();
			return comp;
		}

		public static void SetParentLocal(this Component component, Transform parent) {
			var     transform        = component.transform;
			var     rectTransform    = transform as RectTransform;
			Vector2 offsetMin        = Vector2.zero, offsetMax = Vector2.zero;
			var     hasRectTransform = rectTransform != null;
			if (hasRectTransform) {
				offsetMin = rectTransform.offsetMin;
				offsetMax = rectTransform.offsetMax;
			}

			var pos      = transform.localPosition;
			var scale    = transform.localScale;
			var rotation = transform.localRotation;
			transform.SetParent(parent);
			transform.localPosition = pos;
			transform.localScale    = scale;
			transform.localRotation = rotation;
			if (hasRectTransform) {
				rectTransform.offsetMin = offsetMin;
				rectTransform.offsetMax = offsetMax;
			}
		}

		public static void DestroyAllChildren(this Transform transform) {
			if (Application.isPlaying)
				for (var i = 0; i < transform.childCount; i++)
					Object.Destroy(transform.GetChild(i).gameObject);
			else
				while (transform.childCount > 0)
					Object.DestroyImmediate(transform.GetChild(0).gameObject);
		}

		public static void Reset(this Transform transform) {
			transform.localPosition = Vector3.zero;
			transform.localScale    = Vector3.one;
			transform.localRotation = Quaternion.identity;
		}

		public static void SetLayerRecursive(this GameObject gameObject, int layer) {
			gameObject.layer = layer;
			for (var i = 0; i < gameObject.transform.childCount; i++)
				gameObject.transform.GetChild(i).gameObject.SetLayerRecursive(layer);
		}
	}
}