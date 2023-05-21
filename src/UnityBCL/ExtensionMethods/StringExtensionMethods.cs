using UnityEngine;

namespace UnityBCL {
	public static class StringExtensionMethods {
		public static string Color(this string str, Color color) => $"<color={color.ToHex()}>{str}</color>";
	}
}