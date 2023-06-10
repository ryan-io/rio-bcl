using System.Threading;
using Cysharp.Threading.Tasks;

namespace Procedural {
	public abstract class RegionRemovalSolver {
		public abstract UniTask<int[,]> Remove(int[,] map, CancellationToken token);
	}
}