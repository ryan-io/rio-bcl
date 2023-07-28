using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UnityBCL.Serialization {
#if UNITY_EDITOR || UNITY_STANDALONE
	[Title("Serialization Configuration")]
#endif
	[Serializable]
	public class SerializeConfiguration {
		const string SERIALIZED_DATA_INFO =
			"Verify the seed value in the 'SerializedData' file name match with the Mapsolver seed.";

		const string SAVE_ROOT_INFO =
			"This folder is nested under 'SaveFolderRoot'. Any data saved will be in 'SaveFolder/Root/SaveFolder/your-file'";

		public static readonly string DefaultGameObjectName = "AppPathsDefinitions";

		public static readonly string NoAppPathDefinitionsFound =
			"AppPathDefinitions was not found in scene. A new GO has been created.";

		public static readonly string Explorer = "explorer.exe";

		public static readonly string NoDirectoryFound =
			"Directory does not exist. The codebase needs to be debugged. There are fallbacks to create the directory if it cannot be found.";

#if UNITY_EDITOR || UNITY_STANDALONE
		[Title("Folder Setup")]
		[field: InfoBox(SAVE_ROOT_INFO)]
#endif
		[field: SerializeField]
		public string SaveFolderRoot { get; set; } = "SerializedData";

#if UNITY_EDITOR || UNITY_STANDALONE
		[field: InfoBox("Where serialized data is saved")]
#endif
		[field: SerializeField]
		public string SaveFolder { get; set; } = "MyData";

#if UNITY_EDITOR || UNITY_STANDALONE
		[field: ValueDropdown("GetFormats")]
#endif
		[field: SerializeField]
		public string SaveFormat { get; private set; } = ".txt";

		static IEnumerable GetFormats() {
			var formats = new HashSet<string> {
				".txt",
				".json",
				".bytes",
				".prefab"
			};

			return formats;
		}
	}
}