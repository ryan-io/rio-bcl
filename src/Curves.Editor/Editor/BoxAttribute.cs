// using System;
// using Sirenix.OdinInspector.Editor;
// using Sirenix.Utilities.Editor;
// using UnityEditor;
// using UnityEngine;
//
// namespace Curves.Editor {
// 	public class DarkBoxAttribute : Attribute {
// 		public readonly bool withBorders;
//
// 		public DarkBoxAttribute() {
// 		}
//
// 		public DarkBoxAttribute(bool withBorders) {
// 			this.withBorders = withBorders;
// 		}
// 	}
//
// 	public class ColorBox : Attribute {
// 	}
//
// 	[DrawerPriority(0, 99)]
// 	public class ColorBoxDrawer : OdinAttributeDrawer<ColorBox> {
// 		protected override void DrawPropertyLayout(GUIContent label) {
// 			int hashCode = Property.ValueEntry.TypeOfValue.Name.GetHashCode();
// 			var h        = (float)((hashCode + (double)int.MaxValue) / uint.MaxValue);
// 			_color   = Color.HSVToRGB(h, 0.95f, 0.75f);
// 			_color.a = 0.15f;
// 			BoxGUI.BeginBox(_color);
// 			CallNextDrawer(label);
// 			BoxGUI.EndBox();
// 		}
//
// 		const float GOLDEN_RATIO = 0.618033988749895f;
// 		Color       _color;
// 	}
// 	#if UNITY_EDITOR
//
// 	[DrawerPriority(0, 99)]
// 	public class DarkBoxDrawer : OdinAttributeDrawer<DarkBoxAttribute> {
// 		protected override void DrawPropertyLayout(GUIContent label) {
// 			BoxGUI.BeginBox(new Color(0, 0, 0, 0.15f));
// 			CallNextDrawer(label);
//
// 			BoxGUI.EndBox(Attribute.withBorders ? Color : (Color?)null);
// 		}
//
// 		public static readonly Color Color = EditorGUIUtility.isProSkin
// 			                                     ? Color.Lerp(Color.black, Color.white, 0.1f)
// 			                                     : Color.gray;
// 	}
//
// 	internal static class BoxGUI {
// 		public static void BeginBox(Color color) {
// 			currentLayoutRect = EditorGUILayout.BeginVertical(SirenixGUIStyles.None);
//
// 			if (Event.current.type == EventType.Repaint)
// 				SirenixEditorGUI.DrawSolidRect(currentLayoutRect, color);
// 		}
//
// 		public static void EndBox(Color? borders = null) {
// 			EditorGUILayout.EndVertical();
//
// 			if (Event.current.type == EventType.Repaint && borders != null)
// 				SirenixEditorGUI.DrawBorders(currentLayoutRect, 1, 1, 1, 1, borders.Value);
//
// 			GUILayout.Space(1);
// 		}
//
// 		static Rect currentLayoutRect;
// 	}
// 	#endif
// }

