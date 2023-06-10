using System;

namespace Procedural {
	[Serializable]
	public enum ProgressState {
		Pending,
		Starting,
		PopulatingMap,
		SmoothingMap,
		RemovingRegions,
		CreatingBoundary,
		ScalingGrid,
		PreparingAndSettingTiles,
		GeneratingMesh, // mesh collider is generated during this state1
		CalculatingPathfinding,
		CompilingData,
		Disposing,
		Complete
	}
}