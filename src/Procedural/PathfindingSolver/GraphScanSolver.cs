using System.Collections;
using System.Threading;
using Pathfinding;
using UnityBCL;
using UnityEngine;

namespace Procedural {
	public static class GraphScanSolver {
		public static void RemoveGraphs() {
			var data = GetAstarActiveData();

			if (data == null || data.graphs.Length < 1)
				return;

			var count = data.graphs.Length;

			for (var i = 0; i < count; i++) {
				if (data.graphs[i] == null)
					continue;

				data.RemoveGraph(data.graphs[i]);
			}
		}

		public static void ScanGraph(NavGraph graph) => AstarPath.active.Scan(graph);

		public static IEnumerator ScanGraphAsync(NavGraph graph, CancellationToken token,
			ProceduralController controller) {
			Physics.SyncTransforms();

#if UNITY_STANDALONE || UNITY_EDITOR
			var log = new UnityLogging();
#endif

			foreach (var progress in AstarPath.active.ScanAsync(graph)) {
#if UNITY_EDITOR || UNITY_STANDALONE

				log.Msg("Scanning... " + progress.description + " - " + (progress.progress * 100).ToString("0") +
				        $"%					{controller.GetTimeElapsedInMilliseconds                   / 1000f}  s. elapsed",
					"Procedural Generation - Pathfinding");
#endif

				if (token.IsCancellationRequested) {
#if UNITY_EDITOR || UNITY_STANDALONE
					log.Msg("Pathfinding cancelled...");
#endif
					yield break;
				}

				yield return null;
			}
#if UNITY_EDITOR || UNITY_STANDALONE
			log.Msg($"Pathfinding Complete!					{controller.GetTimeElapsedInMilliseconds / 1000}  s. elapsed ",
				"Procedural Generation - Pathfinding",
				true,
				true);
#endif
		}

		public static AstarData GetAstarActiveData() {
			AstarPath.FindAstarPath();
			var data = AstarPath.active.data;
			return data;
		}
	}
}