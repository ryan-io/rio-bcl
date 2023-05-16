// OmniBCL

namespace OmniBCL.Logging; 

public interface ILogger {
	object   Context         { get; }
	bool     IsEnabled       { get; }
	LogLevel DefaultLogLevel { get; }
	void     Log(string message);
	void     Log(LogLevel logLevel, string message);
}