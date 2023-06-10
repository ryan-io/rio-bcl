using System;
using System.Collections.Generic;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine;

namespace Procedural {
	public static class GridGraphBuilder {
		const string AIPathfindingName = "AI Ground Level Grid";

		public static GridGraph BuildGridGraph(AstarData astarData, Data buildData) {
			var graph = astarData.AddGraph(typeof(GridGraph)) as GridGraph;

			if (graph == null)
				throw new Exception("Error - could not cast GridGraph.");

			graph.name = AIPathfindingName;

			SetGraph(graph, buildData);
			return graph;
		}

		static void SetGraph(GridGraph graph, Data buildData) {
			graph.collision.use2D    = true;
			graph.cutCorners         = true;
			graph.is2D               = true;
			graph.rotation           = new Vector3(90f, 0, 0);
			graph.collision.diameter = buildData.CollisionDiameter;
			graph.collision.mask     = LayerMask.GetMask(buildData.ObstacleLayerMask.ToArray());

			var graphDimensions = DetermineGraphDimensions(buildData);
			graph.SetDimensions(graphDimensions.x, graphDimensions.y, buildData.GridNodeSize);
		}

		static int2 DetermineGraphDimensions(Data buildData) {
			// A* 2D is XZ
			var targetWidth  = buildData.Dimensions.x * buildData.CellSize;
			var targetHeight = buildData.Dimensions.y * buildData.CellSize;

			var sizeX = Mathf.RoundToInt(targetWidth  / buildData.GridNodeSize);
			var sizeY = Mathf.RoundToInt(targetHeight / buildData.GridNodeSize);
			return new int2(sizeX, sizeY);
		}

		public readonly struct Data {
			public string       Name              { get; }
			public int2         Dimensions        { get; }
			public float        CollisionDiameter { get; }
			public List<string> ObstacleLayerMask { get; }
			public float        GridNodeSize      { get; }
			public float        CellSize          { get; }

			public Data(string name, int2 dimensions, float collisionDiameter, List<string> obstacleLayerMask,
				float gridNodeSize, float cellSize) {
				Name              = name;
				Dimensions        = dimensions;
				CollisionDiameter = collisionDiameter;
				ObstacleLayerMask = obstacleLayerMask;
				GridNodeSize      = gridNodeSize;
				CellSize          = cellSize;
			}
		}
	}
}