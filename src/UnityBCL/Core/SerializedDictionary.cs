using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UnityBCL {
	[Serializable]
	[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout, KeyLabel = "")]
	public class SerializedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
		where TKey : notnull {
		// Internal
		[SerializeField] [AssetList] List<KeyValuePair> list = new();

#pragma warning disable 0414
		[SerializeField] [HideInInspector] bool keyCollision;
#pragma warning restore 0414
		[SerializeField] [HideInInspector] Dictionary<TKey, TValue> _dict = new();

		[SerializeField] Dictionary<TKey, int> _indexByKey = new();

		// IDictionary
		public TValue this[TKey key] {
			get => _dict[key];
			set {
				_dict[key] = value;

				if (_indexByKey.ContainsKey(key)) {
					var index = _indexByKey[key];
					list[index] = new KeyValuePair(key, value);
				}
				else {
					list.Add(new KeyValuePair(key, value));
					_indexByKey.Add(key, list.Count - 1);
				}
			}
		}

		public ICollection<TKey>   Keys   => _dict.Keys;
		public ICollection<TValue> Values => _dict.Values;

		public void Add(TKey key, TValue value) {
			_dict.Add(key, value);
			list.Add(new KeyValuePair(key, value));
			_indexByKey.Add(key, list.Count - 1);
		}

		public bool ContainsKey(TKey key) => _dict.ContainsKey(key);

		public bool Remove(TKey key) {
			if (_dict.Remove(key)) {
				var index = _indexByKey[key];
				list.RemoveAt(index);
				_indexByKey.Remove(key);
				return true;
			}

			return false;
		}

		public bool TryGetValue(TKey key, out TValue value) {
			if (!ContainsKey(key)) Debug.LogWarning($"ERROR: No key found matching {key}.");

			return _dict.TryGetValue(key, out value!);
		}

		// ICollection
		public int  Count      => _dict.Count;
		public bool IsReadOnly { get; set; }

		public void Add(KeyValuePair<TKey, TValue> pair) {
			Add(pair.Key, pair.Value);
		}

		public void Clear() {
			_dict.Clear();
			list.Clear();
			_indexByKey.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> pair) => _dict.TryGetValue(pair.Key, out var value) &&
		                                                         EqualityComparer<TValue>.Default.Equals(value,
			                                                         pair.Value);

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			if (array == null)
				throw new ArgumentException("The array cannot be null.");
			if (arrayIndex < 0)
				throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
			if (array.Length - arrayIndex < _dict.Count)
				throw new ArgumentException("The destination array has fewer elements than the collection.");

			foreach (var pair in _dict) {
				array[arrayIndex] = pair;
				arrayIndex++;
			}
		}

		public bool Remove(KeyValuePair<TKey, TValue> pair) {
			if (_dict.TryGetValue(pair.Key, out var value)) {
				var valueMatch = EqualityComparer<TValue>.Default.Equals(value, pair.Value);
				if (valueMatch) return Remove(pair.Key);
			}

			return false;
		}

		// IEnumerable
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dict.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();

		// Since lists can be serialized natively by unity no custom implementation is needed
		public void OnBeforeSerialize() {
		}

		// Fill dictionary with list pairs and flag key-collisions.
		public void OnAfterDeserialize() {
			_dict.Clear();
			_indexByKey.Clear();
			keyCollision = false;

			for (var i = 0; i < list.Count; i++) {
				var key = list[i].Key;
				if (!ContainsKey(key)) {
					_dict.Add(key, list[i].Value);
					_indexByKey.Add(key, i);
				}
				else {
					keyCollision = true;
				}
			}
		}

		public Dictionary<TKey, TValue> GetNonSerializedDictionary() => _dict;

		public ReadOnlyDictionary<TKey, TValue> GetNonSerializedReadonlyDictionary() {
			var roDict = new ReadOnlyDictionary<TKey, TValue>(_dict);
			return roDict;
		}

		// Serializable KeyValuePair struct
		[Serializable]
		struct KeyValuePair {
			[LabelText("Dictionary Key")] [LabelWidth(250f)]
			public TKey Key;

			public TValue Value;

			public KeyValuePair(TKey Key, TValue Value) {
				this.Key   = Key;
				this.Value = Value;
			}
		}
	}
}