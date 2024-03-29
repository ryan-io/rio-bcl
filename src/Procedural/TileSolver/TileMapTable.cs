﻿using System;
using Sirenix.OdinInspector;
using UnityBCL;
using UnityEngine.Tilemaps;

namespace Procedural {
	[Serializable]
	[TableList]
	[DictionaryDrawerSettings(KeyLabel = "Map Id", ValueLabel = "Map", DisplayMode = DictionaryDisplayOptions.OneLine)]
	public class TileMapTable : SerializedDictionary<TileMapType, Tilemap> {
		public bool IsMapObjectActive(Tilemap tilemap) => tilemap.gameObject.activeSelf;

		public void SetMapActive(Tilemap tilemap) {
			if (IsMapObjectActive(tilemap))
				return;

			tilemap.gameObject.SetActive(true);
		}

		public void SetMapInactive(Tilemap tilemap) {
			if (IsMapObjectActive(tilemap))
				return;

			tilemap.gameObject.SetActive(false);
		}
	}
}