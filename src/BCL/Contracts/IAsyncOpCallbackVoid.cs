// BCL

using System;
using System.Threading;

namespace RIO.BCL {
	/// <summary>
	///  Abstraction for void async operation callbacks.
	/// </summary>
	public interface IAsyncOpCallbackVoid {
		Action<CancellationToken> Context { get; set; }
		void                      Alert(CancellationToken token);
	}
}