using System;
using System.Collections;
using System.Collections.Generic;
using BCL;
using UnityEngine;

namespace UnityBCL {
	public class PoolQueue : IEnumerable<Queue<GameObject>> {
		readonly PoolingDictionary _poolingDictionary = new();
		public   int               QueueCount => _poolingDictionary.Count;

		public IEnumerator<Queue<GameObject>> GetEnumerator() {
			foreach (var queue in _poolingDictionary.Values) yield return queue;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public static bool CannotPool(PooledObjectSetup pooledObject, int quantity) => quantity < 1
			|| pooledObject.Asset                                                               == null
			|| !pooledObject.Asset.RuntimeKeyIsValid();

		public void AddToQueueDictionary(PooledObjectSetup pooledObject, Queue<GameObject> queue) {
			if (!_poolingDictionary.ContainsKey(pooledObject.PoolIdentifier))
				_poolingDictionary.Add(pooledObject.PoolIdentifier, queue);
		}

		public Queue<GameObject> GetQueue(string objectIdentifier, bool outputLogs = false) {
			var hasValue = _poolingDictionary.TryGetValue(objectIdentifier, out var queue);

			if (outputLogs) {
				var log = new UnityLogging(this);
				log.Log(LogLevel.Test, $"Queue key: {queue}");
				log.Log(LogLevel.Test, $"Queue key: {hasValue}");
			}

			return (hasValue ? queue : default)!;
		}

		public void Enqueue(string poolId, GameObject obj, bool outputLogs = false) {
			var queue = GetQueue(poolId, outputLogs);

			// if (Application.isEditor) {
			// 	var log = new UnityLogging(this);
			// 	log.Log(LogLevel.Warning, $"Could not find queue with the identifier {poolId}");
			// }

			queue.Enqueue(obj);
		}

		public GameObject Dequeue(string objectIdentifier, bool outputLogs) {
			var queue = GetQueue(objectIdentifier, outputLogs);

			if (queue == null) {
				if (Application.isEditor) {
					var log = new UnityLogging(this);
					log.Log(LogLevel.Warning, $"Could not find queue for {objectIdentifier}");
				}

				return default!;
			}

			return queue.Dequeue();
		}

		[Obsolete]
		bool GetObjectFromQueue(string poolId, GameObject obj, out Queue<GameObject> queue, bool outputLogs = false) {
			queue = GetQueue(poolId, outputLogs);

			if (queue.Count < 1)
				return true;

			var containsObj = queue.Contains(obj);

			if (!containsObj)
				return true;
			return false;
		}
	}
}