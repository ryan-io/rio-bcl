using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Procedural {
	public class TileHashset : HashSet<TileData> {
		public TileData this[Vector2Int location]
			=> this.FirstOrDefault(t => t.Location == location);
	}
}