using System.Threading;
using RIO.BCL;
using Cysharp.Threading.Tasks;

namespace Procedural {
	public class CellularAutomataFillMapSolver : FillMapSolver {
		readonly MapSolverModel _model;

		public CellularAutomataFillMapSolver(MapSolverModel model) => _model = model;

		public override async UniTask<int[,]> Fill(int[,] map, CancellationToken token)
			=> await UniTask.RunOnThreadPool(
				   () => RunDeterministicFill(map, token), cancellationToken: token);

		async UniTask<int[,]> RunDeterministicFill(int[,] map, CancellationToken token) {
			var pseudoRandom = CreateRandom();
			var mapCopy      = (int[,])map.Clone();

			await UniTask.RunOnThreadPool(
				() => {
					for (var x = 0; x < _model.MapWidth; x++)
						for (var y = 0; y < _model.MapHeight; y++)
							map[x, y] = DetermineWallFill(mapCopy, x, y, pseudoRandom);
				}, cancellationToken: token);

			return map;
		}

		WeightedRandom<int> CreateRandom() {
			var items = new[] {
				new WeightedRandom<int>.Entry
					{ Item = 1, AccumulatedWeight = _model.WallFillPercentage },
				new WeightedRandom<int>.Entry
					{ Item = 0, AccumulatedWeight = 100 - _model.WallFillPercentage }
			};

			var pseudoRandom = new WeightedRandom<int>(_model.Seed);
			pseudoRandom.AddRange(items);

			return pseudoRandom;
		}

		int DetermineWallFill(int[,] map, int x, int y, WeightedRandom<int> pseudoRandom)
			=> Utility.IsBoundary(_model.MapWidth, _model.MapHeight, x, y) ? 1 : pseudoRandom.Pop();
	}
}