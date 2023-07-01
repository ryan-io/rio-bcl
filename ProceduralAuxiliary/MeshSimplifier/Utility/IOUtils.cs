using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace ProceduralAuxiliary.MeshSimplifier.Utility {
	public static class IOUtils {
		public static string MakeSafeRelativePath(string path) {
			if (string.IsNullOrEmpty(path))
				return null;

			path = path.Replace('\\', '/').Trim('/');

			if (Path.IsPathRooted(path))
				throw new ArgumentException("The path cannot be rooted.", "path");

			// Make the path safe
			var pathParts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			for (var i = 0; i < pathParts.Length; i++) pathParts[i] = MakeSafeFileName(pathParts[i]);
			return string.Join("/", pathParts);
		}

		public static string MakeSafeFileName(string name) {
			var invalidFileNameChars = Path.GetInvalidFileNameChars();

			var sb             = new StringBuilder(name.Length);
			var lastWasInvalid = false;
			for (var i = 0; i < name.Length; i++) {
				var c = name[i];
				if (!invalidFileNameChars.Contains(c)) {
					sb.Append(c);
				}
				else if (!lastWasInvalid) {
					lastWasInvalid = true;
					sb.Append('_');
				}
			}

			return sb.ToString();
		}

		internal static void CreateParentDirectory(string path) {
#if UNITY_EDITOR 
			var lastSlashIndex = path.LastIndexOf('/');
			if (lastSlashIndex != -1) {
				var parentPath = path.Substring(0, lastSlashIndex);
				if (!AssetDatabase.IsValidFolder(parentPath)) {
					lastSlashIndex = parentPath.LastIndexOf('/');
					if (lastSlashIndex != -1) {
						var folderName       = parentPath.Substring(lastSlashIndex + 1);
						var folderParentPath = parentPath.Substring(0, lastSlashIndex);
						CreateParentDirectory(parentPath);
						AssetDatabase.CreateFolder(folderParentPath, folderName);
					}
					else {
						AssetDatabase.CreateFolder(string.Empty, parentPath);
					}
				}
			}
			#endif
		}

		internal static bool DeleteEmptyDirectory(string path) {
#if UNITY_EDITOR
			
			var deletedAllSubFolders = true;
			var subFolders           = AssetDatabase.GetSubFolders(path);
			for (var i = 0; i < subFolders.Length; i++)
				if (!DeleteEmptyDirectory(subFolders[i]))
					deletedAllSubFolders = false;

			if (!deletedAllSubFolders)
				return false;
			if (!AssetDatabase.IsValidFolder(path))
				return true;

			string[] assetGuids = AssetDatabase.FindAssets(string.Empty, new[] { path });
			if (assetGuids.Length > 0)
				return false;

			return AssetDatabase.DeleteAsset(path);
#endif
		}
	}
}