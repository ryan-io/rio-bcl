// OmniBCL

namespace BCL {
	/// <summary>
	///  Internal logging abstraction. This is not the same as the logging abstraction in the Microsoft.Extensions.Logging namespace.
	/// </summary>
	public interface ILogging {
		object   Context         { get; set; }
		bool     IsEnabled       { get; }
		LogLevel DefaultLogLevel { get; }
		void     Log(string message);
		void     Log(LogLevel logLevel, string message);
	}
}