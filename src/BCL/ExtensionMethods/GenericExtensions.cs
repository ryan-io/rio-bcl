// OmniBCL

namespace BCL {
	public static class GenericExtensions {
		public static void Empty<T>(this T?[] original) {
			var count = original.Length;

			for (var i = 0; i < count; i++) original[i] = default;
		}
	}
}