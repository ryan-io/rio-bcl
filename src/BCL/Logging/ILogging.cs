// OmniBCL

namespace BCL.Logging {
	public interface ILogging {
		object   Context         { get; set; }
		bool     IsEnabled       { get; }
		LogLevel DefaultLogLevel { get; }
		void     Log(string message);
		void     Log(LogLevel logLevel, string message);
	}
}