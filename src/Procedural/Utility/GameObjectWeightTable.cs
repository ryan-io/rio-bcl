using System;
using UnityBCL;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Procedural {
	[Serializable]
	public class GameObjectWeightTable : SerializedDictionary<GameObject, double> {
	}

	[Serializable]
	public class TileWeightTable : SerializedDictionary<TileBase, double> {
	}
}