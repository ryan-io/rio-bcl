// BCL

namespace BCL {
	/// <summary>
	///  Logging provider abstraction. Used in Unity systems.
	/// </summary>
	public interface ILoggingProvider {
		void Msg(string message, string ctx = "", bool bold = false, bool italic = false, int size = 15);
		void Warning(string message, string ctx = "", bool bold = false, bool italic = false, int size = 16);
		void Error(string message, string ctx = "", bool bold = false, bool italic = false, int size = 16);
		void Test(string message, string ctx = "", bool bold = true, bool italic = true, int size = 18);
	}
}