using RIO.BCL.Serialization;
using UnityEngine;

namespace UnityBCL.Serialization {
	public class UnitySaveLocation : SaveLocation {
		public static UnitySaveLocation GetDefault => new(SERIALIZED_DATA_FOLDER);

		public const string SERIALIZED_DATA_FOLDER = "SerializedData";

		/// <summary>
		/// The combined location that takes into account the Unity Application path.
		/// </summary>
		public string SaveLocation {
			get {
				EnsureDirectoryExists();
				return Path;
			}
		}

		/// <summary>
		/// The combined location that does not take into account the Unity Application Path.
		/// This method is used when generating string paths to assets within the context of Unity.
		/// This will not add the Unity Application Path to the path string.
		/// </summary>
		/// <returns></returns>
		public string SaveLocationRaw {
			get {
				EnsureDirectoryExists();
				return ASSETS + Folder;
			}
		}

		/// <summary>
		/// Creates and returns a new string for a path to a file. A file name and file format are required.
		/// If there is some sort of identifier or tag you would like to include, set parameter filePrefix.
		/// This returns a complete path with respect to SaveLocation.
		/// </summary>
		/// <param name="fileName">Name of file</param>
		/// <param name="fileFormat">.json, .txt, .prefab, etc.</param>
		/// <param name="filePrefix">Give more context to what this file is</param>
		/// <returns>New string that is the path to your file</returns>
		public string GetFilePath(string fileName, string fileFormat, string filePrefix = "") {
			if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(fileFormat))
				return string.Empty;

			filePrefix = string.IsNullOrWhiteSpace(filePrefix) ? string.Empty : filePrefix + UNDERSCORE;
			return SaveLocation + BACKSLASH + filePrefix + fileName + fileFormat;
		}

		public string GetFilePathRaw(string fileName, string fileFormat, string filePrefix = "") {
			if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(fileFormat))
				return string.Empty;

			filePrefix = string.IsNullOrWhiteSpace(filePrefix) ? string.Empty : filePrefix + UNDERSCORE;
			return SaveLocationRaw + BACKSLASH + filePrefix + fileName + fileFormat;
		}

		/// <summary>
		/// Creates a string for the full file path.
		/// </summary>
		string Path => $"{Directory}{BACKSLASH}{Folder}";

		/// <summary>
		/// Root folder within Unity Application path. Defined in constructor.
		/// </summary>
		string Folder { get; }

		/// <summary>
		/// Helper method that will ensure 'Folder' exists. Takes into account Unity Application path.
		/// </summary>
		void EnsureDirectoryExists() {
			Serializer.EnsureDirectoryExists(Folder, Directory);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="folder">Folder name. Do not add Unity Application path. Ex: "SerializedData", "Prefabs", etc.</param>
		public UnitySaveLocation(string folder) : base(UnityApplicationPath) {
			Folder = folder;
		}

		static readonly string UnityApplicationPath = Application.dataPath;
		const           string BACKSLASH            = "/";
		const           string UNDERSCORE           = "_";
		const           string ASSETS               = "Assets/";
	}
}