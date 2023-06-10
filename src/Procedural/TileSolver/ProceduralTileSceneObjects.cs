using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Procedural {
	[Serializable]
	public class ProceduralTileSceneObjects {
		[SerializeField] [Required] Grid _gridObject;

		[Title("Tile Mapping")] [SerializeField]
		TileMapTable _tileMapTable;

		[SerializeField] TileTable _outlineTileTable;

		public Grid GridObject => _gridObject;

		public TileMapTable TileMapTable     => _tileMapTable;
		public TileTable    OutlineTileTable => _outlineTileTable;
	}
}