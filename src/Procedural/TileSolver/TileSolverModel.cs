namespace Procedural {
	public readonly struct TileSolverModel {
		public TileHashset                          TileHashset          { get; }
		public TileWeightDictionary                 TileWeightDictionary { get; }
		public ProceduralTilePlacementConfiguration TileConfig           { get; }
		public ProceduralTileSceneObjects           TileObjects          { get; }
		public int                                  MapWidth             { get; }
		public int                                  MapHeight            { get; }
		public int                                  CellSize             { get; }


		public TileSolverModel(ProceduralTileSolverMonobehaviorModel monoModel, ProceduralTileSolverModel model,
			int mapWidth, int mapHeight, int cellSize) {
			MapWidth             = mapWidth;
			MapHeight            = mapHeight;
			CellSize             = cellSize;
			TileHashset          = model.TileHashset;
			TileWeightDictionary = model.TileWeightDictionary;
			TileConfig           = monoModel.ProceduralTilePlacementConfig;
			TileObjects          = monoModel.TileMapGameObjects;
		}
	}
}