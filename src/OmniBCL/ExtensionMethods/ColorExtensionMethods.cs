using UnityEngine;

namespace OmniBCL.ExtensionMethods; 

public static class ColorExtensionMethods {
	public static string ToHex(this Color color) => $"#{color.r.ToByte():X2}{color.g.ToByte():X2}{color.b.ToByte():X2}";
}