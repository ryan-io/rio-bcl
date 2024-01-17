// BCL.Serialization

namespace RIO.BCL.Serialization {
	public abstract class SaveLocation {
		public string Directory { get; }

		public SaveLocation(string directory) {
			Directory = directory;
		}
	}
}