using Test.Code;

namespace Test.Environment; 

public class TestLogMarshalSizeOf {
	public void Test_LogSizeOfMarshalObjectProperties() {
		new LogMarshalSizeOf().WriteSizeOfMarshalObjectPropertiesToConsole(new MarshalObject(100, 100, 2));
	}
}