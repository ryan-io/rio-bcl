using System.Threading;
using Cysharp.Threading.Tasks;

namespace Procedural {
	public abstract class SmoothMapSolver {
		public abstract UniTask<int[,]> Smooth(int[,] map, CancellationToken token);
	}
}