using System;
using System.IO;

namespace RIO.BCL {
	public static class TextLogger {
		public static void ValidateLogFile(string path) {
			if (File.Exists(path))
				return;

			lock (_mutex) {
				if (!File.Exists(path)) {
					string createText = "Log: " + Environment.NewLine;
					File.WriteAllText(path, createText);
				}
			}
		}

		public static void Write(string msg, string path) {
			lock (_mutex) {
				File.AppendAllText(path, GetFormattedDateTime() + ":   " + msg + Environment.NewLine);
			}
		}
	
		static string GetFormattedDateTime() => DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

		static readonly object _mutex = new();
	}
}