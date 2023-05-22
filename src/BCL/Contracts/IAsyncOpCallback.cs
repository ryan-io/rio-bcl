using System;
using System.Threading;

namespace BCL {
	public interface IAsyncOpCallback<T> {
		Action<T, CancellationToken> Context { get; set; }
		void          Alert(T ctx, CancellationToken token);
	}
}