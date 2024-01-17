// UnityBCL.Serialization

namespace RIO.BCL.Serialization {
	public static class SerializeJob {
		public readonly struct Json {
			public string SaveName { get; }
			public string SavePath { get; }

			public Json(string saveName, string savePath = "") {
				SaveName = saveName;
				SavePath = savePath;
			}
		}

		public readonly struct JsonBytes {
			public string SaveName { get; }
			public string SavePath { get; }
			public byte[] Data     { get; }

			public JsonBytes(byte[] data, string saveName, string savePath) {
				Data     = data;
				SavePath = savePath;
				SaveName = saveName;
			}
		}

		public readonly struct Text {
			public string SaveName { get; }
			public string SavePath { get; }
			public byte[] Data     { get; }

			public Text(string saveName, byte[] data, string savePath = "") {
				SaveName = saveName;
				Data     = data;
				SavePath = savePath;
			}
		}
	}
}