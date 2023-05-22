// BCL

using System;
using System.Threading;

namespace BCL {
	public interface IAsyncOpCallbackVoid {
		Action<CancellationToken> Context { get; set; }
		void                      Alert(CancellationToken token);
	}
}