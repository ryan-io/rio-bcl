using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ProceduralAuxiliary.ProceduralCollider {
	public static class PolygonColliderEditorExtention {
		public static void DrawIcon(GameObject gameObject, int idx) {
#if UNITY_EDITOR || UNITY_STANDALONE
			var largeIcons = GetTextures("sv_label_", string.Empty, 0, 8);
			var icon       = largeIcons[idx];
			SetIcon(gameObject, icon.image as Texture2D);
#endif
		}

		static void SetIcon(GameObject go, Texture2D image) {
#if UNITY_EDITOR|| UNITY_STANDALONE
#if UNITY_2021_2_OR_NEWER
            EditorGUIUtility.SetIconForObject(go, image);
#else
			var egu   = typeof(EditorGUIUtility);
			var flags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
			var args  = new object[] { go, image };
			var setIcon = egu.GetMethod("SetIconForObject", flags, null,
				new[] { typeof(Object), typeof(Texture2D) }, null);
			setIcon.Invoke(null, args);
#endif
#endif
		}

		static GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count) {
#if UNITY_EDITOR|| UNITY_STANDALONE
			var array = new GUIContent[count];
			for (var i = 0; i < count; i++)
				array[i] = EditorGUIUtility.IconContent(baseName + (startIndex + i) + postFix);

			return array;
#endif
		}
	}
}