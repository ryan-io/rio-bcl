namespace Procedural {
	public readonly struct BorderAndBoundsSolverModel {
		public int[,] MapBorder     { get; }
		public int    MapBorderSize { get; }

		public BorderAndBoundsSolverModel(ProceduralMapModel model, ProceduralMapConfiguration config) {
			MapBorder     = model.BorderMap;
			MapBorderSize = config.MapBorderSize;
		}
	}
}