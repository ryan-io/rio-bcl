using System.Threading;
using Cysharp.Threading.Tasks;

namespace Procedural {
	public abstract class BorderAndBoundsSolver {
		public abstract UniTask<int[,]> Determine(int[,] borderMap, int[,] map, CancellationToken token);
	}
}