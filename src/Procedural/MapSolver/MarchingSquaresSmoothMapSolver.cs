using System.Threading;
using Cysharp.Threading.Tasks;
using UnityBCL;

namespace Procedural {
	public class MarchingSquaresSmoothMapSolver : SmoothMapSolver {
		readonly MapSolverModel _model;

		int[,] _cachedMap;

		public MarchingSquaresSmoothMapSolver(MapSolverModel model) => _model = model;
		UnityLogging Logger { get; } = new();

		public override async UniTask<int[,]> Smooth(int[,] map, CancellationToken token) {
			_cachedMap = map;
			var mapCopy = (int[,])map.Clone();

			for (var i = 0; i < _model.SmoothingIterations; i++)
				map = await GetSmoothedMap(mapCopy, token);

			return map;
		}

		async UniTask<int[,]> GetSmoothedMap(int[,] mapCopy, CancellationToken token) {
			await UniTask.Run(
				() => {
					for (var x = 0; x < _model.MapWidth; x++) {
						for (var y = 0; y < _model.MapHeight; y++)
							if (Process(ref mapCopy, x, y, token))
								break;
					}
				}, cancellationToken: token);

			return mapCopy;
		}

		bool Process(ref int[,] mapCopy, int x, int y, CancellationToken token) {
			if (token.IsCancellationRequested) {
				Logger.Warning("Cancellation of generation was requested.", "Async Cancellation");
				return true;
			}

			mapCopy = DetermineNeighborLimits(x, y, mapCopy);
			return false;
		}


		int[,] DetermineNeighborLimits(int x, int y, int[,] mapCopy) {
			var surroundingWalls = GetAdjacentWallsCount(x, y);

			if (surroundingWalls > _model.UpperNeighborLimit)
				mapCopy[x, y] = 1;

			else if (surroundingWalls < _model.LowerNeighborLimit)
				mapCopy[x, y] = 0;

			return mapCopy;
		}

		int GetAdjacentWallsCount(int x, int y) {
			var count = 0;

			for (var neighborX = x - 1; neighborX <= x + 1; neighborX++) {
				for (var neighborY = y - 1; neighborY <= y + 1; neighborY++)
					count = DetermineCount(x, y, neighborX, neighborY, count);
			}

			return count;
		}

		int DetermineCount(int gridX, int gridY, int neighborX, int neighborY, int count) {
			if (IsWithinBoundary(neighborX, neighborY)) {
				if (neighborX != gridX || neighborY != gridY)
					count += _cachedMap[neighborX, neighborY];
			}

			else {
				count++;
			}

			return count;
		}

		bool IsWithinBoundary(int neighborX, int neighborY)
			=> neighborX >= 0 && neighborX < _model.MapWidth && neighborY >= 0 && neighborY < _model.MapHeight;
	}
}