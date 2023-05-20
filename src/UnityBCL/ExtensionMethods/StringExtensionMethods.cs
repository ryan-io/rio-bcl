using UnityEngine;

namespace UnityBCL.ExtensionMethods {
	public static class StringExtensionMethods {
		public static string Color(this string str, Color color) => $"<color={color.ToHex()}>{str}</color>";
	}
}