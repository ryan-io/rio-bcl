using System.Collections.Generic;
using RIO.BCL;
using UnityEngine;

namespace ProceduralAuxiliary.PoissonSpawning {
	public interface IPoissonObjectHandler {
		IEnumerable<GameObject> SpawnObjects(
			List<Vector2> points, WeightedRandom<GameObject> objects, Transform spawnLocation);

		WeightedRandom<GameObject> GetWeightedRandom(PoissonWeightTable table);
	}
}