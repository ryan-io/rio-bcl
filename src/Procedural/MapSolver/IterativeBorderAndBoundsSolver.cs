using System.Threading;
using Cysharp.Threading.Tasks;

namespace Procedural {
	public class IterativeBorderAndBoundsSolver : BorderAndBoundsSolver {
		readonly BorderAndBoundsSolverModel _dataModel;

		readonly MapSolverModel _solverModel;


		public IterativeBorderAndBoundsSolver(MapSolverModel solverModel, BorderAndBoundsSolverModel dataModel) {
			_solverModel = solverModel;
			_dataModel   = dataModel;
		}

		public override async UniTask<int[,]> Determine(int[,] borderMap, int[,] map, CancellationToken token) {
			await UniTask.RunOnThreadPool(
				() => {
					var lengthX = _dataModel.MapBorder.GetLength(0);
					var lengthY = _dataModel.MapBorder.GetLength(1);

					for (var x = 0; x < lengthX; x++) {
						for (var y = 0; y < lengthY; y++)
							borderMap[x, y] = DetermineIfTileIsBorder(borderMap, map, x, y);
					}
				}, cancellationToken: token);

			return borderMap;
		}

		int DetermineIfTileIsBorder(int[,] borderMapCopy, int[,] map, int x, int y) {
			if (IsBorder(x, y))
				return map[x - _dataModel.MapBorderSize, y - _dataModel.MapBorderSize];

			return borderMapCopy[x, y] = 1;
		}

		bool IsBorder(int x, int y) => x >= _dataModel.MapBorderSize                        &&
		                               x < _solverModel.MapWidth + _dataModel.MapBorderSize &&
		                               y >= _dataModel.MapBorderSize                        &&
		                               y < _solverModel.MapHeight + _dataModel.MapBorderSize;
	}
}