using System.IO;
using RIO.BCL;
using UnityEngine;

namespace UnityBCL.Serialization {
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

			fileName += Strings.ASSET_SUFFIX;

			if (GuardAgainstNoDirectory(_saveFolder))
				CreateNewDirectory(_saveFolder);

			var path               = _saveFolder + fileName;
			var assetAlreadyExists = AssetWithSameNameExists(path);

			if (assetAlreadyExists && !overWrite) {
				_log.Log(LogLevel.Warning, Strings.ASSET_EXISTS);
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

			_log.Log(Strings.SAVE_SUCCESS + path);
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
			=> _log.Log(Strings.SIMPLE_SAVE);

		void LogIsNullOrWhitespace()
			=> _log.Log(Strings.SIMPLE_SAVE);

		void LogReturningEarly()
			=> _log.Log(Strings.SIMPLE_SAVE);

		static bool GuardAgainstNullOrWhitespace(string path)
			=> string.IsNullOrWhiteSpace(path);

		bool GuardAgainstNoDirectory(string path)
			=> !DirectoryExists(_saveFolder + path);

#region PLUMBING

		readonly string   _saveFolder;
		readonly ILogging _log;

		public GenericSaver(ILogging logger, string saveFolder) {
			_log = logger;

			if (GuardAgainstNullOrWhitespace(saveFolder))
				_saveFolder = Strings.ASSETS_FOLDER + Strings.DEFAULT_SAVE_FOLDER;

			else
				_saveFolder = Strings.ASSETS_FOLDER + saveFolder + Strings.SLASH;
		}

		static class Strings {
			public const string SLASH             = "/";
			public const string ASSET_SUFFIX       = ".asset";
			public const string ASSETS_FOLDER      = "Assets/";
			public const string DEFAULT_SAVE_FOLDER = "Save/";

			public const string INVALID_PATH =
				"Cannot create asset. The provided string was null or contained only whitespace.";

			public const string RETURNING_EARLY = "Saver will not exit early...";

			public const string OBJECT_IS_NULL =
				"There object you are attempting to save is null or not valid. Please try again.";

			public const string PATH_PREFIX          = "Attempting to save asset at: ";
			public const string CREATING_DIRECTORIES = "Creating required directories...";

			public const string ASSET_EXISTS =
				"Asset with the same name already exists. Please choose a different name.";

			public const string SAVE_SUCCESS = "Asset was saved successfully to the following directory path: ";
			public const string SIMPLE_SAVE  = "SimpleSave";
		}

#endregion
	}
}