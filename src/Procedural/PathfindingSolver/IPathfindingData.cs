using System.Collections.Generic;
using UnityEngine;

namespace Procedural {
	public interface IPathfindingData {
		List<Vector3> TilePositions               { get; }
		List<Vector3> TilePositionsShifted        { get; }
		int           NodesCheckedThatWereNotNull { get; }
		int           NodesCheckThatWereNull      { get; }
		int           NodesWithTile               { get; }
		int           NodesWithoutTile            { get; }
	}
}