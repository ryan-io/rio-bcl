using System.IO;
using BCL;
using UnityBCL;
using UnityEngine;

namespace Unity.Serialization.Serialization {
	public class GenericSaver : ISave {
		public void Save(Object obj, string fileName, bool overWrite = false) {
			if (GuardAgainstNullObject(obj)) {
				LogObjectIsNull();
				LogReturningEarly();

				return;
			}

			if (GuardAgainstNullOrWhitespace(_saveFolder)) {
				LogIsNullOrWhitespace();
				LogReturningEarly();

				return;
			}

			fileName += Strings.AssetSuffix;

			if (GuardAgainstNoDirectory(_saveFolder))
				CreateNewDirectory(_saveFolder);

			var path               = _saveFolder + fileName;
			var assetAlreadyExists = AssetWithSameNameExists(path);

			if (assetAlreadyExists && !overWrite) {
				_log.Log(LogLevel.Warning, Strings.AssetExists);
				return;
			}

			try {
#if UNITY_EDITOR

				UnityEditor.AssetDatabase.CreateAsset(obj, path);
				UnityEditor.AssetDatabase.SaveAssets();

#endif
			}
			catch (DirectoryNotFoundException e) {
				_log.Log(LogLevel.Error, "Could not save at the defined path. Please debug.");
				_log.Log(LogLevel.Error, e.Message);
				return;
			}

			_log.Log(Strings.SaveSuccess + path);
		}

		static bool GuardAgainstNullObject(Object o) => !o;

		static void CreateNewDirectory(string path) {
			Directory.CreateDirectory(path);
		}

		static bool DirectoryExists(string path)
			=> Directory.Exists(path);

		static bool AssetWithSameNameExists(string path)
			=> File.Exists(path);

		void LogObjectIsNull()
			=> _log.Log(Strings.SimpleSave);

		void LogIsNullOrWhitespace()
			=> _log.Log(Strings.SimpleSave);

		void LogReturningEarly()
			=> _log.Log(Strings.SimpleSave);

		static bool GuardAgainstNullOrWhitespace(string path)
			=> string.IsNullOrWhiteSpace(path);

		bool GuardAgainstNoDirectory(string path)
			=> !DirectoryExists(_saveFolder + path);

#region PLUMBING

		readonly string   _saveFolder;
		readonly ILogging _log;

		public GenericSaver(string saveFolder) {
			_log = new UnityLogging(this);

			if (GuardAgainstNullOrWhitespace(saveFolder))
				_saveFolder = Strings.AssetsFolder + Strings.DefaultSaveFolder;

			else
				_saveFolder = Strings.AssetsFolder + saveFolder + Strings.Slash;
		}

		static class Strings {
			public const string Slash             = "/";
			public const string AssetSuffix       = ".asset";
			public const string AssetsFolder      = "Assets/";
			public const string DefaultSaveFolder = "Save/";

			public const string InvalidPath =
				"Cannot create asset. The provided string was null or contained only whitespace.";

			public const string ReturningEarly = "Saver will not exit early...";

			public const string ObjectIsNull =
				"There object you are attempting to save is null or not valid. Please try again.";

			public const string PathPrefix          = "Attempting to save asset at: ";
			public const string CreatingDirectories = "Creating required directories...";

			public const string AssetExists =
				"Asset with the same name already exists. Please choose a different name.";

			public const string SaveSuccess = "Asset was saved successfully to the following directory path: ";
			public const string SimpleSave  = "SimpleSave";
		}

#endregion
	}
}