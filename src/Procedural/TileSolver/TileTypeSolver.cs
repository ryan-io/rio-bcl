using System.Threading;
using Cysharp.Threading.Tasks;

namespace Procedural {
	public abstract class TileTypeSolver {
		public abstract UniTask SetTiles(int[,] map, CancellationToken token);
	}
}