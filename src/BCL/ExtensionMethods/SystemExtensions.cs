// OmniBCL

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace BCL {
	public static class SystemExtensions {
		public static bool NextBool(this Random r, int truePercentage = 50) => r.NextDouble() < truePercentage / 100.0;

		public static TimeSpan SpanSeconds(this float f) => TimeSpan.FromSeconds(f);

		public static TimeSpan SpanMilSeconds(this float f) => TimeSpan.FromMilliseconds(f);

		public static TimeSpan SpanSeconds(this int f) => TimeSpan.FromSeconds(f);

		public static TimeSpan SpanMilSeconds(this int f) => TimeSpan.FromMilliseconds(f);

		public static string GetMethodThatThrew(this Exception e, out MethodBase? methodBase) {
			var stackTrace = new StackTrace(e);
			var assembly   = Assembly.GetExecutingAssembly();

			var methods = (stackTrace.GetFrames() ?? Array.Empty<StackFrame>())
			             .Select(f => f.GetMethod())
			             .Where(m => m.Module.Assembly == assembly)
			             .ToArray();

			methodBase = methods[^1];

			return methodBase == null ? ERROR : methodBase.Name;
		}

		const string ERROR =
			"ERROR -> could not get method name from reflection. " +
			"This may be be a result of an incorrect invoke from an undefined assembly";
	}
}