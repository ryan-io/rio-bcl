using System;
using System.Collections.Generic;
using System.Threading;
using BCL;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityBCL {
	public static class CoreExtensions {
#region OBJECT

		public static IEnumerable<T> InstantiateMany<T>(T obj, int number) where T : Object {
			if (number < 0) {
				yield break;
			}

			for (var i = 0; i < number; i++) {
				yield return Object.Instantiate(obj);
			}
		}

		public static I[] FindComponentsWithInterface<T, I>() where T : Object, I where I : class {
			var outputList = new List<I>();

			if (outputList.IsNullOrEmpty())
				return Array.Empty<I>();

			var sceneComponents = Object.FindObjectsOfType<T>();

			foreach (var obj in sceneComponents) {
				if (obj is I castedObj) {
					outputList.Add(castedObj);
				}
			}

			return outputList.ToArray();
		}

#endregion

#region GAMEOBJECT

		public static void Face(this GameObject go, GameObject target) {
			var dir   = target.transform.position - go.transform.position;
			var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			go.transform.rotation = Quaternion.Euler(0, 0, angle);
		}

		public static I GetComponentInterface<T, I>(this GameObject o) where T : I {
			if (o.TryGetComponent<T>(out var extractedType) && extractedType is I)
				return extractedType;

			return default!;
		}

		public static bool TryGetComponentInterface<T, I>(this GameObject o, out I component) where T : I {
			component = default!;

			if (!o.TryGetComponent<T>(out var extractedType) || extractedType is not I)
				return false;

			component = extractedType;
			return true;
		}

		public static void SetLayerWithDepth(this GameObject o, int layer) {
			var components = o.GetComponentsInChildren<Transform>();

			if (components.IsNullOrEmpty())
				return;

			for (var i = 0; i < components.Length; i++) {
				if (!components[i])
					continue;

				components[i].gameObject.layer = layer;
			}
		}

#endregion

#region COMPONENT

		public static I GetComponentInterface<T, I>(this Component o) where T : I {
			if (o.TryGetComponent<T>(out var extractedType) && extractedType is I)
				return extractedType;

			return default!;
		}

#endregion

#region TRANSFORM

		public static void Face(this Transform tr, Transform target) {
			var dir   = target.position - tr.position;
			var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			tr.rotation = Quaternion.Euler(0, 0, angle);
		}

#endregion

#region LAYERMASK

		// public static bool ContainsColliderMask(this LayerMask lM, Collider colliderToCheck) {
		// 	return lM.ContainsMask(colliderToCheck.gameObject.layer);
		// }

		public static bool DoesNotContainsLayer(this LayerMask mask, int layerToCheckFor) {
			return mask != (mask | (1 << layerToCheckFor));
		}

		public static bool ContainsMask(this LayerMask mask, int layer) {
			return mask == (mask | (1 << layer)); // shift the bits to our layer bit, check if mask contains
		}

#endregion

#region UNITASK

		public static async UniTask SecAsTask(this float f, CancellationToken token) {
			await UniTask.Delay(TimeSpan.FromSeconds(f), cancellationToken: token);
		}

		public static async UniTask SecAsTask(this int integer, CancellationToken token) {
			await UniTask.Delay(TimeSpan.FromSeconds(integer), cancellationToken: token);
		}

		public static async UniTask FramesAsTask(this int integer, CancellationToken token) {
			await UniTask.DelayFrame(integer, cancellationToken: token);
		}

#endregion
	}
}