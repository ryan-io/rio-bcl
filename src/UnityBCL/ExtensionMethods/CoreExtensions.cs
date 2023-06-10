using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace UnityBCL {
	public static class CoreExtensions {
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

#region OBJECT

		public static IEnumerable<T> InstantiateMany<T>(T obj, int number) where T : Object {
			if (number < 0) yield break;

			for (var i = 0; i < number; i++) yield return Object.Instantiate(obj);
		}

		public static I[] FindComponentsWithInterface<T, I>() where T : Object, I where I : class {
			var outputList = new List<I>();

			if (outputList.IsNullOrEmpty())
				return Array.Empty<I>();

			var sceneComponents = Object.FindObjectsOfType<T>();

			foreach (var obj in sceneComponents)
				if (obj is I castedObj)
					outputList.Add(castedObj);

			return outputList.ToArray();
		}

#endregion

#region GAMEOBJECT

		public static void DeleteObj(this IEnumerable<Component> c, GameObject? owner = null) {
			foreach (var component in c)
				if (component && component.gameObject) {
#if UNITY_EDITOR || UNITY_STANDALONE
					if (component.gameObject == owner)
						continue;

					if (owner)
						Object.DestroyImmediate(component.gameObject);
					else
						Object.DestroyImmediate(component);
#else
					if (owner)
						Object.Destroy(component.gameObject);
					else
						Object.Destroy(component);
#endif
				}
		}

		public static void ZeroPosition(this GameObject o) => o.transform.position = Vector3.zero;

		public static void MakeStatic(this GameObject o, bool recursive) {
			if (!recursive) {
				o.isStatic = true;
				return;
			}

			var objects = o.GetComponentsInChildren<Transform>();

			for (var i = 0; i < objects.Length; i++)
				objects[i].gameObject.isStatic = true;
		}

		public static void SetLayerRecursive(this GameObject o, int mask) {
			var objects = o.GetComponentsInChildren<Transform>();

			for (var i = 0; i < objects.Length; i++) objects[i].gameObject.SetLayer(mask);
		}

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

#region LAYERMASK

		// public static bool ContainsColliderMask(this LayerMask lM, Collider colliderToCheck) {
		// 	return lM.ContainsMask(colliderToCheck.gameObject.layer);
		// }

		public static bool DoesNotContainsLayer(this LayerMask mask, int layerToCheckFor)
			=> mask != (mask | (1 << layerToCheckFor));

		public static bool ContainsMask(this LayerMask mask, int layer)
			=> mask == (mask | (1 << layer)); // shift the bits to our layer bit, check if mask contains

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

#region UNSORTED

		public static List<T>? GetPropertyValues<T>(this object? o, bool addNull = true) where T : class {
			if (o == null)
				return default;

			var properties = o.GetType().GetProperties();
			var output     = new List<T>();

			foreach (var p in properties) {
				var value = p.GetValue(o) as T;

				if (value == null || !addNull)
					continue;

				output.Add(value);
			}

			return output;
		}

		public static bool HasNullProperty<T>(this T obj) {
			return typeof(T).GetProperties().All(propertyInfo => propertyInfo.GetValue(obj) != null);
		}

		public static T FixComponent<T>(this GameObject owner) where T : Component {
			var hasComponent = owner.TryGetComponent(typeof(T), out var component);

			if (!hasComponent)
				component = owner.AddComponent<T>();

			return (T)component;
		}

		const byte BytesToGigabytes = (byte)1e-9;

		const BindingFlags CopyComponentFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
		                                        BindingFlags
			                                       .Default | BindingFlags.DeclaredOnly;

		static readonly Random Random = new();

		public static void DeleteFromScene<T>(this IEnumerable<T> e, GameObject owner, bool destroyGameObject = false)
			where T :
			Component {
			foreach (var c in e) {
				if (c.gameObject == owner)
					continue;

				if (Application.isEditor) {
					if (destroyGameObject)
						Object.DestroyImmediate(c.gameObject);
					else
						Object.DestroyImmediate(c);
				}
				else {
					if (destroyGameObject)
						Object.Destroy(c.gameObject);
					else
						Object.Destroy(c);
				}
			}
		}

		public static bool NextBool(this Random r, int truePercentage = 50) => r.NextDouble() < truePercentage / 100.0;

		public static T? CopyComponent<T>(this Component c, GameObject gameObjectToCopyTo) where T : Component {
			var copyComponent       = gameObjectToCopyTo.AddComponent(typeof(T));
			var copyComponentFields = typeof(T).GetProperties(CopyComponentFlags);

			foreach (var p in copyComponentFields) {
				if (!p.CanWrite)
					continue;

				try {
					p.SetValue(c, p.GetValue(copyComponent, null), null);
				}
				catch {
					// ignored
				}

				var fields = typeof(T).GetFields(CopyComponentFlags);

				foreach (var field in fields) field.SetValue(c, field.GetValue(copyComponent));
			}

			return copyComponent as T;
		}

		public static bool IsNullOrEmpty<TKey, TValue>(this Dictionary<TKey, TValue>? dict)
			=> dict == null || !dict.GetEnumerator().MoveNext();

		public static bool IsNullOrEmpty<T>(this T[]? collection)
			=> collection == null || !collection.GetEnumerator().MoveNext();

		public static bool IsNullOrEmpty<T>(this List<T>? list)
			=> list == null || !list.GetEnumerator().MoveNext();

		public static bool IsNullOrEmpty(this IEnumerable? enumerable)
			=> enumerable == null || !enumerable.GetEnumerator().MoveNext();

		public static bool IsNullOrEmpty<T>(this IEnumerable<T>? enumerable) => enumerable == null || !enumerable.Any();

		public static bool IsNull(this object? obj) => obj == null;

		public static int ToIntNonZero(this bool b) => b ? 1 : -1;

		public static float Clamp0(this float f) {
			if (f < 0)
				f = 0;

			return f;
		}

		public static T? GetComponentRecursive<T>(this GameObject gameObject)
			where T : Component {
			var comp = gameObject.GetComponent<T>();

			if (!comp)
				comp = gameObject.GetComponentInChildren<T>(true);

			if (comp) return comp;

			var compArray = gameObject.GetComponentsInChildren<T>();

			if (compArray == null || compArray.Length < 1) return default;

			return compArray[0];
		}

		public static T? GetComponentRecursive<T>(this GameObject gameObject, out Transform? parent)
			where T : Component {
			var comp = gameObject.GetComponent<T>();

			if (!comp)
				comp = gameObject.GetComponentInChildren<T>(true);

			if (comp) {
				parent = comp.transform;
				return comp;
			}

			var compArray = gameObject.GetComponentsInChildren<T>();

			if (compArray == null || compArray.Length < 1) {
				parent = null;
				return default;
			}

			parent = compArray[0].transform;
			return compArray[0];
		}

		public static GameObject New(this GameObject o, string name) => new(name);

		public static float Sq(this float f) => f * f;

		public static int Sq(this int i) => i * i;

		public static double Sq(this double d) => d * d;

		public static byte ToGb(this byte b) => (byte)(b * BytesToGigabytes);

		public static decimal ToGb(this long l) => l * BytesToGigabytes;

		public static double NextRange(this Random random, double min, double max)
			=> new Random().NextDouble() * (max - min) + min;

		public static bool IsNullOrEmpty(this ICollection enumerable)
			=> enumerable == null || enumerable.Count < 1;

		public static void SetEulerAngles(this Transform tr, Vector3 angles) {
			tr.eulerAngles = angles;
		}

		public static void SetLocalEulerAngles(this Transform tr, Vector3 angles) {
			tr.localEulerAngles = angles;
		}

		public static IEnumerable Concatenate<T>(this List<T> list, params IEnumerable[] lists) {
			var concatList = new List<string>();
			foreach (var enumerable in lists) {
				foreach (var member in enumerable) concatList.Add(member.ToString());
			}

			return concatList;
		}

		public static void OnComplete(this UniTask task, Action continuation) {
			task.GetAwaiter().OnCompleted(continuation);
		}

		public static Task LoopAsync<T>(this IEnumerable<T> list, Func<T, Task> function)
			=> Task.WhenAll(list.Select(function));

		public static void FisherYatesShuffle<T>(this IList<T> list) {
			var n     = list.Count;
			var count = list.Count;
			while (n > 1) {
				n--;
				var k = new Random().Next(n + 1);
				(list[k], list[n]) = (list[n], list[k]);
			}
		}

		public static void CheckListForValueAddIfDoesNotContain<TValue>(IReadOnlyList<TValue> value,
			ICollection<TValue> list) {
			Debug.Assert(value != null && list != null && value.Count > 0);
			if (value != null)
				for (var i = 0; i < value.Count; i++) {
					if (list != null && !list.Contains(value[i])) list.Add(value[i]);
					Debug.Log("TESTING:" + " " + value[i]);
				}
		}

		public static void RemoveComponent<T>(this GameObject gameObject) where T : Component {
			var component = gameObject.GetComponent<T>();
			if (component != null) Object.Destroy(component);
			else Debug.LogWarning($"GameObject does not contain component of type: {typeof(T)}");
		}

		public static T[] GetComponentsWithTag<T>(this GameObject gameObject, string tag) where T : Component {
			var componentList = new List<T>();
			var t             = gameObject.transform;

			if (!t)
				return default!;

			foreach (Transform tr in t) {
				if (!tr.CompareTag(tag)) continue;
				componentList.Add(tr.GetComponent<T>());
			}

			return componentList.ToArray();
		}

		public static T[] GetComponentsInParentWithTag<T>(this GameObject gameObject, string tag) where T : Component {
			var componentList = new List<T>();
			var t             = gameObject.transform.parent;

			if (!t)
				return default!;

			foreach (Transform tr in t) {
				if (!tr.CompareTag(tag)) continue;
				componentList.Add(tr.GetComponent<T>());
			}

			return componentList.ToArray();
		}

		public static T[] GetComponentsInChildWithTag<T>(this GameObject gameObject, string tag) where T : Component {
			var componentList = new List<T>();
			var t             = gameObject.GetComponentsInChildren<Transform>();

			if (t == null || t.Length < 1)
				return default!;

			foreach (var tr in t) {
				if (!tr.CompareTag(tag)) continue;
				componentList.Add(tr.GetComponent<T>());
			}

			return componentList.ToArray();
		}

		public static T GetComponentInChildWithTag<T>(this GameObject parent, string tag) where T : Component {
			var t = parent.transform;

			if (!t)
				return default!;

			foreach (Transform tr in t)
				if (tr.CompareTag(tag))
					return tr.GetComponent<T>();

			return null!;
		}

		public static GameObject GetTransformInChildWithTag(this GameObject parent, string tag) {
			var t = parent.transform;

			if (!t)
				return default!;

			foreach (Transform tr in t)
				if (tr.CompareTag(tag))
					return tr.GetComponent<Transform>().gameObject;

			return null!;
		}

		public static TValue?
			GetValueOrDefault<TKey, TValue>(this ReadOnlyDictionary<TKey, TValue> dictionary, TKey key) {
			var hasValue = dictionary.TryGetValue(key, out var value);
			return hasValue ? value : default;
		}

		public static TValue? GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) {
			var hasValue = dictionary.TryGetValue(key, out var value);
			return hasValue ? value : default;
		}

		public static TValue? GetValueOrDefault<TKey, TValue>(
			this SerializedDictionary<TKey, TValue> inspectorDictionary,
			TKey key) where TKey : notnull {
			var hasValue = inspectorDictionary.TryGetValue(key, out var value);
			return hasValue ? value : default;
		}

		public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
			IReadOnlyDictionary<TKey, TValue> range) {
			if (range.IsNullOrEmpty())
				return;

			foreach (var pair in range)
				dictionary.Add(pair.Key, pair.Value);
		}

		public static void AddWithCheck<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
			TValue value) {
			if (dictionary.ContainsKey(key)) {
				Debug.Log($"Dictionary already contains {key}. ");
				return;
			}

			dictionary.Add(key, value);
		}

		public static void SetLayer(this GameObject o, string layerName) => o.layer = LayerMask.NameToLayer(layerName);
		public static void SetLayer(this GameObject o, int mask)         => o.layer = mask;

		public static void SetLayerRecursive(this GameObject o, string layerName, bool includeInactive, params
			Transform[] toIgnore) {
			var trs = o.GetComponentsInChildren<Transform>(includeInactive);

			if (trs == null || trs.Length < 1)
				return;

			var mask = LayerMask.NameToLayer(layerName);

			foreach (var tr in trs) {
				if (toIgnore.Contains(tr))
					continue;

				tr.gameObject.layer = mask;
			}
		}

		public static void SetLayerRecursive(this GameObject o, int mask, int maskToIgnore, bool includeInactive) {
			var trs = o.GetComponentsInChildren<Transform>(includeInactive);

			if (trs == null || trs.Length < 1)
				return;

			foreach (var tr in trs)
				tr.gameObject.layer = mask;
		}

#endregion
	}
}