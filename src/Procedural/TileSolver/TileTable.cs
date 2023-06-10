using System;
using Sirenix.OdinInspector;
using UnityBCL;
using UnityEngine.Tilemaps;

namespace Procedural {
	[Serializable]
	[TableList]
	[DictionaryDrawerSettings(KeyLabel = "Id", ValueLabel = "Tile", DisplayMode = DictionaryDisplayOptions.OneLine)]
	public class TileTable : SerializedDictionary<string, TileBase> {
		public new TileBase this[string id] {
			get {
				var contains = ContainsKey(id);
				return !contains ? null : this[id];
			}
		}
	}
}