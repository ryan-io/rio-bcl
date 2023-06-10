using Unity.Mathematics;
using UnityEngine;

namespace Procedural {
	public struct GridSetEvent {
		public ProceduralTileSceneObjects SceneObjects { get; }
		public int2                       MapSize      { get; }
		public int                        Cellsize     { get; }

		public GridSetEvent(ProceduralTileSceneObjects sceneObjects, int2 mapSize, int cellSize) {
			SceneObjects = sceneObjects;
			MapSize      = mapSize;
			Cellsize     = cellSize;
		}
	}

	public static class GridUtil {
		public static void SetGridOrigin(ProceduralTileSceneObjects sceneObjects, int2 mapSize) {
			if (sceneObjects == null || sceneObjects.GridObject == null)
				return;

			sceneObjects.GridObject.gameObject.transform.position = ProcessNewPosition(mapSize);
		}

		static Vector3 ProcessNewPosition(int2 mapSize) => new(
			Mathf.CeilToInt(-mapSize.x  / 2f),
			Mathf.FloorToInt(-mapSize.y / 2f),
			0);

		public static void SetGridScale(ProceduralTileSceneObjects sceneObjects, int cellSize) {
			if (sceneObjects == null || sceneObjects.GridObject == null)
				return;

			sceneObjects.GridObject.gameObject.transform.localScale = ProcessNewScale(cellSize);
		}

		static Vector3 ProcessNewScale(int cellSize) => new(
			cellSize,
			cellSize,
			cellSize);
	}
}