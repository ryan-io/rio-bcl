using UnityEngine;

namespace Procedural {
	public class SquareGrid {
		public readonly Square[,] Squares;

		public SquareGrid(int[,] map, float squareSize) {
			var nodeCountX   = map.GetLength(0);
			var nodeCountY   = map.GetLength(1);
			var tileWidth    = nodeCountX * squareSize;
			var tileHeight   = nodeCountY * squareSize;
			var controlNodes = new ControlNode[nodeCountX, nodeCountY];

			for (var i = 0; i < nodeCountX; i++) {
				for (var j = 0; j < nodeCountY; j++) {
					var position = new Vector2(
						-tileWidth  / 2f + i * squareSize + squareSize / 2f,
						-tileHeight / 2f + j * squareSize + squareSize / 2f);
					controlNodes[i, j] = new ControlNode(position, map[i, j] == 1, squareSize);
				}
			}

			Squares = new Square[nodeCountX - 1, nodeCountY - 1];

			for (var i = 0; i < nodeCountX - 1; i++) {
				for (var j = 0; j < nodeCountY - 1; j++)
					Squares[i, j] = new Square(
						controlNodes[i, j + 1],
						controlNodes[i    + 1, j + 1],
						controlNodes[i    + 1, j],
						controlNodes[i, j]);
			}
		}
	}
}