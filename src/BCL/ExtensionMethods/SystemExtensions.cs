// OmniBCL

using System;

namespace BCL.ExtensionMethods {
	public static class SystemExtensions {
		public static bool NextBool(this Random r, int truePercentage = 50) {
			return r.NextDouble() < truePercentage / 100.0;
		}

		public static TimeSpan SpanSeconds(this float f) {
			return TimeSpan.FromSeconds(f);
		}

		public static TimeSpan SpanMilSeconds(this float f) {
			return TimeSpan.FromMilliseconds(f);
		}

		public static TimeSpan SpanSeconds(this int f) {
			return TimeSpan.FromSeconds(f);
		}

		public static TimeSpan SpanMilSeconds(this int f) {
			return TimeSpan.FromMilliseconds(f);
		}
	}
}