using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UnityBCL.Serialization.Core {
	[Serializable]
	[Title("Serialization Configuration")]
	public class SerializeConfiguration {
		const string SerializedDataInfo =
			"Verify the seed value in the 'SerializedData' file name match with the Mapsolver seed.";

		const string SaveRootInfo =
			"This folder is nested under 'SaveFolderRoot'. Any data saved will be in 'SaveFolder/Root/SaveFolder/your-file'";

		public static readonly string DefaultGameObjectName = "AppPathsDefinitions";

		public static readonly string NoAppPathDefinitionsFound =
			"AppPathDefinitions was not found in scene. A new GO has been created.";

		public static readonly string Explorer = "explorer.exe";

		public static readonly string NoDirectoryFound =
			"Directory does not exist. The codebase needs to be debugged. There are fallbacks to create the directory if it cannot be found.";

		[Title("Folder Setup")]
		[field: SerializeField]
		[field: InfoBox(SaveRootInfo)]
		public string SaveFolderRoot { get; set; } = "SerializedData";

		[field: SerializeField]
		[field: InfoBox("Where serialized data is saved")]
		public string SaveFolder { get; set; } = "MyData";

		[field: SerializeField]
		[field: ValueDropdown("GetFormats")]
		public string SaveFormat { get; private set; } = ".txt";

		static IEnumerable GetFormats() {
			var formats = new HashSet<string> {
				".txt",
				".json",
				".bytes",
				".omni"
			};

			return formats;
		}
	}
}