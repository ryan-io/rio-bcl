namespace Procedural {
	public readonly struct MapProcessorDto {
		public ProceduralMapConfiguration Configuration { get; }

		public int[,] Map { get; }

		public MapProcessorDto(ProceduralMapConfiguration configuration, int[,] map) {
			Configuration = configuration;
			Map           = map;
		}
	}
}