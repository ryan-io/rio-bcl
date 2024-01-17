// UnityBCL.Serialization

namespace RIO.BCL.Serialization {
	public static class Info {
		public readonly struct Json {
			public string Path          { get; }
			public bool   WasSuccessful { get; }
			public bool   DidOverwrite  { get; }

			public Json(string path, bool wasSuccessful, bool didOverwrite) {
				Path          = path;
				WasSuccessful = wasSuccessful;
				DidOverwrite  = didOverwrite;
			}
		}
	}
}