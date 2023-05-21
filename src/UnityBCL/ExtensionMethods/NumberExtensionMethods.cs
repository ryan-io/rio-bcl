using UnityEngine;

namespace UnityBCL {
	public static class NumberExtensionMethods {
		public static byte ToByte(this float value) {
			value = Mathf.Clamp01(value);
			return (byte)(value * 255);
		}
	}
}