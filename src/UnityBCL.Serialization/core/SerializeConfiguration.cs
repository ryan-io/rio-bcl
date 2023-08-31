using System;
using NaughtyAttributes;
using UnityEngine;

namespace UnityBCL.Serialization {
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

		[field: InfoBox("Where serialized data is saved"), Header("Serialization Configuration")]
		[field: SerializeField]
		public string SaveFolder { get; set; } = "MyData";

		[field: Dropdown("GetFormats")]
		[field: SerializeField]
		public string SaveFormat { get; private set; } = ".txt";

		static DropdownList<string> GetFormats() {
			var formats = new DropdownList<string> {
				{ "JSON", ".json" },
				{ "Text", ".txt" },
				{ "Byte Array", ".bytes" },
				{ "Unity Prefab", ".prefab" },
			};

			return formats;
		}
	}
}