// OmniBCL

namespace RIO.BCL {
	public static class LogStrings {
		public static readonly string LogStr     = "Log: ".Size(16).Italic();
		public static readonly string WarningStr = "Warning: ".Italic().Size(16);
		public static readonly string ErrorStr   = "Error: ".Bold().Size(16);
		public static readonly string TestStr    = "Testing: ".Bold().Italic();

		public static readonly string InvalidString =
			"String is not valid. Please input a valid string.".Bold().Italic().Size(16);

		public static string GetNotEnabledOutput(int id) => $"Logging is currently not enabled for logger ID {id}";
	}
}