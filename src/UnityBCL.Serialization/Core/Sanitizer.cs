using System;
using System.Text.RegularExpressions;

namespace UnityBCL.Serialization.Core {
	public static class Sanitizer {
		const string RegexPattern   = @"[^\w\.@-]";
		const float  DefaultTimeout = 1.0f;

		/// <summary>
		///     If the regex process times out or is null/empty, Sanitize will return string.empty.
		///     This is intended to be used as a comparison. Each sanitized string should check against string.empty.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string Sanitize(string value) {
			var sanitizedValue = InternalRegex(value);
			return sanitizedValue;
		}

		static string InternalRegex(string value) {
			try {
				var process = Regex.Replace(value, RegexPattern, "", RegexOptions.None,
					TimeSpan.FromSeconds(DefaultTimeout));

				return string.IsNullOrWhiteSpace(process) ? string.Empty : process;
			}

			catch (RegexMatchTimeoutException) {
				return string.Empty;
			}
		}
	}
}