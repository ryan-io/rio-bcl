using UnityBCL;

namespace Procedural {
	public interface IMapHandler {
		void Inject(RoomData data, ProceduralMapModel mapData, int seed);
	}

	public class ProceduralMapHandler : Singleton<ProceduralMapHandler, ProceduralMapHandler> {
		RoomProcessor             _processor;
		public RoomData           Data    { get; private set; }
		public ProceduralMapModel MapData { get; private set; }
		public string             Seed    { get; private set; }

		public void DrawRoom() => _processor.Draw();

		public void Inject(RoomData data, ProceduralMapModel mapData, int seed) {
			var dto = new RoomProcessorDto(data, seed, mapData);
			Data       = data;
			_processor = new RoomProcessor(dto);
			MapData    = mapData;
			Seed       = seed.ToString();
		}
	}
}