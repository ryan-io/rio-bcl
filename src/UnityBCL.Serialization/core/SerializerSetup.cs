using BCL.Serialization;
using NaughtyAttributes;
using UnityEngine;

namespace UnityBCL.Serialization {
	/// <summary>
	///     The 'brain' of saving & loading persistent data within the Engine environment.
	/// </summary>
	public class SerializerSetup : MonoBehaviour {
		[SerializeField] [Label("")] SerializeConfiguration _config                  = null!;

		public static readonly string DefaultDirectory = Application.dataPath + "/";

		/// <summary>
		///     Save location. This lies within the Unity Application Data Path directory.
		///     The serializedfield '_saveFolderName' should default to "Saves'.
		///     There is currently no need to change this name, but it is exposed in the inspector if data should
		///     be saved in a different location that "Saves".
		///     All strings will be sanitized.
		///     This is NOT encrypted as of 17 March, 2022.
		/// </summary>

		Serializer Serializer { get; } = new ();

		public string SaveLocation {
			get {
				Serializer.EnsureDirectoryExists(SavePath, _config.SaveFolder);
				return SavePath;
			}
		}

		public string SaveFolderRaw => _config.SaveFolder;

		public string FileFormat => _config.SaveFormat;

		string SavePath => DefaultDirectory + $"{_config.SaveFolder}/";
	}
}