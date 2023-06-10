using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Procedural {
	public class TileMapCompressor : MonoBehaviour {
		[SerializeField] [BoxGroup("0", false)] [Indent]
		GameObject _tileMapRoot;

		[Button]
		public void Compress() {
			var tileMaps = _tileMapRoot.GetComponentsInChildren<Tilemap>(true);

			foreach (var tilemap in tileMaps)
				if (tilemap)
					tilemap.CompressBounds();
		}
	}
}