namespace RIO.BCL {
	public static class StringExtensionMethods {
		public static string Bold(this string str) => "<b>" + str + "</b>";

		public static string Italic(this string str) => "<i>" + str + "</i>";

		public static string Size(this string str, int size) => $"<size={size}>{str}</size>";
	}
}