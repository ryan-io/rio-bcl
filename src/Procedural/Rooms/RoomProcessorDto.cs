namespace Procedural {
	public readonly struct RoomProcessorDto {
		public RoomData           RoomData { get; }
		public int                Seed     { get; }
		public ProceduralMapModel Model    { get; }

		public RoomProcessorDto(RoomData roomData, int seed, ProceduralMapModel model) {
			RoomData = roomData;
			Seed     = seed;
			Model    = model;
		}
	}
}