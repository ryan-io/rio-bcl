using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UnityBCL {
	[HideMonoScript]
	[ExecuteInEditMode]
	public class GameObjectEnabler : Singleton<GameObjectEnabler, IGameObjectEnabler>, IGameObjectEnabler {
		[Title("Game Objects to Enable")]
		[field: SerializeField]
		[field: ListDrawerSettings(ShowFoldout = true)]
		[field: HideLabel]
		public List<GameObject> GameObjectsToEnable { get; private set; } = new();

		void Start() {
			if (GameObjectsToEnable.IsEmptyOrNull())
				return;

			var count = GameObjectsToEnable.Count;

			for (var i = 0; i < count; i++)
				if (GameObjectsToEnable[i] == null)
					GameObjectsToEnable.RemoveAt(i);
		}

		void Update() {
			if (Application.IsPlaying(this))
				return;

			var count = GameObjectsToEnable.Count;

			try {
				for (var i = 0; i < count; i++)
					if (GameObjectsToEnable[i] == null && i < GameObjectsToEnable.Count)
						GameObjectsToEnable.Remove(GameObjectsToEnable[i]);
			}

			catch (ArgumentOutOfRangeException) {
			}
			catch (NullReferenceException) {
			}
		}

		public List<GameObject> GameObjects => GameObjectsToEnable;

		public void Add(GameObject obj) {
			if (!obj || GameObjectsToEnable.Contains(obj))
				return;

			GameObjectsToEnable.Add(obj);
		}

		public void Remove(GameObject obj) {
			if (!obj || !GameObjectsToEnable.Contains(obj))
				return;

			GameObjectsToEnable.Remove(obj);
		}

		public void Enable() {
			if (GameObjectsToEnable.IsEmptyOrNull())
				return;

			var count = 0;

			if (GameObjectsToEnable != null)
				count = GameObjectsToEnable.Count;

			for (var i = 0; i < count; i++) {
				if (GameObjectsToEnable != null) {
					var o = GameObjectsToEnable[i];
					if (o.activeInHierarchy || o == null)
						continue;
				}

				if (GameObjectsToEnable != null) GameObjectsToEnable[i].SetActive(true);
			}
		}
	}
}