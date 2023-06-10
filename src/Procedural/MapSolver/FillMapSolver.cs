using System.Threading;
using Cysharp.Threading.Tasks;

namespace Procedural {
	public abstract class FillMapSolver {
		public abstract UniTask<int[,]> Fill(int[,] map, CancellationToken token);
	}
}