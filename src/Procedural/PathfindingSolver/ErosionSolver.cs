using Cysharp.Threading.Tasks;
using Pathfinding;

namespace Procedural {
	public static class ErosionSolver {
		public static async UniTask Erode(GridGraph graph, Data data) {
			await UniTask.Yield();
			graph.erosionUseTags = data.ErodeNodesAtBoundaries;

			if (!data.ErodeNodesAtBoundaries)
				return;

			graph.erodeIterations = data.NodesToErodeAtBoundaries;
			graph.erosionFirstTag = data.StartingNodeIndexToErode;
		}

		public readonly struct Data {
			public bool ErodeNodesAtBoundaries   { get; }
			public int  NodesToErodeAtBoundaries { get; }
			public int  StartingNodeIndexToErode { get; }

			public Data(bool erodeNodesAtBoundaries, int nodesToErodeAtBoundaries, int startingNodeIndexToErode) {
				ErodeNodesAtBoundaries   = erodeNodesAtBoundaries;
				NodesToErodeAtBoundaries = nodesToErodeAtBoundaries;
				StartingNodeIndexToErode = startingNodeIndexToErode;
			}
		}
	}
}