using System;
using System.Collections.Generic;
using System.Reflection;
using RIO.BCL;
using UnityEngine;

namespace UnityBCL {
	public class UnityLogging : ILogging, ILoggingProvider {
		const string InvalidLogLevelMsg = "An appropriate log level was not defined or was incorreclty passed";

		static readonly object UninitializedContext = new();

		public UnityLogging(LogLevel defaultLogLevel = LogLevel.Normal, bool isEnabled = true) {
			Context         = UninitializedContext;
			DefaultLogLevel = defaultLogLevel;
			IsEnabled       = isEnabled;
		}

		public UnityLogging(object context, LogLevel defaultLogLevel = LogLevel.Normal, bool isEnabled = true) {
			Context         = context;
			DefaultLogLevel = defaultLogLevel;
			IsEnabled       = isEnabled;
		}

		public object   Context         { get; set; }
		public bool     IsEnabled       { get; set; }
		public LogLevel DefaultLogLevel { get; set; }

		public void Log(string message) => Log(DefaultLogLevel, message);

		public void Log(LogLevel logLevel, string message) {
			if (!IsEnabled) return;

			switch (logLevel) {
				case LogLevel.Normal:
					Msg(message);
					break;
				case LogLevel.Warning:
					Warning(message);
					break;
				case LogLevel.Error:
					Error(message);
					break;
				case LogLevel.Critical:
					Error(message, size: 18, bold: true);
					break;
				case LogLevel.Test:
					Test(message);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, InvalidLogLevelMsg);
			}
		}

		public void Msg(string message, string ctx = "", bool bold = false, bool italic = false, int size = 15) {
#if UNITY_EDITOR|| UNITY_STANDALONE
			Output(Sanitize(message, bold, italic, size), LogLevel.Normal, ctx);
#endif
		}

		public void Warning(string message, string ctx = "", bool bold = false, bool italic = false, int size = 16) {
#if UNITY_EDITOR|| UNITY_STANDALONE
			Output(Sanitize(message, bold, italic, size), LogLevel.Warning, ctx);
#endif
		}

		public void Error(string message, string ctx = "", bool bold = false, bool italic = false, int size = 16) {
#if UNITY_EDITOR|| UNITY_STANDALONE
			Output(Sanitize(message, bold, italic, size), LogLevel.Error, ctx);
#endif
		}

		public void Test(string message, string ctx = "", bool bold = true, bool italic = true, int size = 18) {
#if UNITY_EDITOR || UNITY_STANDALONE
			Output(Sanitize(message, bold, italic, size), LogLevel.Test, ctx);
#endif
		}
		
		public string Sanitize(string value, bool bold, bool italic, int size) {
#if UNITY_EDITOR|| UNITY_STANDALONE
			if (string.IsNullOrWhiteSpace(value)) {
				Output(InvalidLogLevelMsg, LogLevel.Error);
				return string.Empty;
			}

			var outPutString = SanitizeMultiLine(value, bold, italic, size);

			return outPutString;
#else
			return string.Empty;
#endif
		}

		static string SanitizeMultiLine(string value, bool bold, bool italic, int size) {
#if UNITY_EDITOR|| UNITY_STANDALONE
			var outPutString = string.Empty;
			var stringSplit  = value.Split(Break().ToCharArray());

			var count = stringSplit.Length;

			for (var i = 0; i < count; i++)
				outPutString = ProcessMember(bold, italic, size, stringSplit, i, outPutString, count);

			return outPutString;
#else
			return string.Empty;
#endif
		}

		static string ProcessMember(bool bold, bool italic, int size, IReadOnlyList<string> stringSplit, int i,
			string outPutString, int count) {
			var addString = stringSplit[i];

			if (bold)
				addString = addString.Bold();

			if (italic)
				addString = addString.Italic();

			addString = addString.Size(size);

			outPutString += addString;

			if (i != count - 1)
				outPutString += Break();

			return outPutString;
		}

		void Output(string value, LogLevel logLevel, string ctx = "") {
			var ctxOutput = " (Ctx: ".Color(Color.magenta) + $"{ctx}) ".Color(Color.cyan);

			switch (logLevel) {
				case LogLevel.Normal:
					OutputNormal(value, ctx, ctxOutput);
					break;
				case LogLevel.Warning:
					OutputWarning(value, ctx, ctxOutput);
					break;
				case LogLevel.Error:
					OutputError(value, ctx, ctxOutput);
					break;
				case LogLevel.Critical:
					OutputCritical(value, ctx, ctxOutput);
					break;
				case LogLevel.Test:
					OutputTest(value, ctx, ctxOutput);
					break;
			}
		}

		void OutputTest(string value, string ctx, string ctxOutput) {
			var output = string.IsNullOrWhiteSpace(ctx)
				             ? Header(LogStrings.TestStr) + value
				             : Header(LogStrings.TestStr) + ctxOutput + value;
			output = OutputContextFooter(output);
			Debug.Log(output);
		}

		void OutputCritical(string value, string ctx, string ctxOutput) {
			var output = string.IsNullOrWhiteSpace(ctx)
				             ? Header(LogStrings.ErrorStr) + value
				             : Header(LogStrings.ErrorStr) + ctxOutput + value;
			output = OutputContextFooter(output);
			Debug.Log(output);
		}

		void OutputError(string value, string ctx, string ctxOutput) {
			var output = string.IsNullOrWhiteSpace(ctx)
				             ? Header(LogStrings.ErrorStr) + value
				             : Header(LogStrings.ErrorStr) + ctxOutput + value;
			output = OutputContextFooter(output);
			Debug.LogError(output);
		}

		void OutputWarning(string value, string ctx, string ctxOutput) {
			var output = string.IsNullOrWhiteSpace(ctx)
				             ? Header(LogStrings.WarningStr) + value
				             : Header(LogStrings.WarningStr) + ctxOutput + value;
			output = OutputContextFooter(output);
			Debug.LogWarning(output);
		}

		void OutputNormal(string value, string ctx, string ctxOutput) {
			var output = string.IsNullOrWhiteSpace(ctx)
				             ? Header(LogStrings.LogStr) + value
				             : Header(LogStrings.LogStr) + ctxOutput + value;
			output = OutputContextFooter(output);
			Debug.Log(output);
		}

		string OutputContextFooter(string output) {
			output += "_________________________ Timestamp: " + DateTime.Now.ToString("dddd, dd MMMM yyyy");
			output += Footer("Context: " + Context.GetType().Name);
			return output;
		}

		static string Break() => "\r\n";

		static string Header(string value) => value;

		static string Footer(string value) => Break() + value;
	}
}