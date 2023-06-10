// OmniBCL

using System;

namespace BCL {
	public static class SystemExtensions {
		public static bool NextBool(this Random r, int truePercentage = 50) => r.NextDouble() < truePercentage / 100.0;

		public static TimeSpan SpanSeconds(this float f) => TimeSpan.FromSeconds(f);

		public static TimeSpan SpanMilSeconds(this float f) => TimeSpan.FromMilliseconds(f);

		public static TimeSpan SpanSeconds(this int f) => TimeSpan.FromSeconds(f);

		public static TimeSpan SpanMilSeconds(this int f) => TimeSpan.FromMilliseconds(f);
	}
}