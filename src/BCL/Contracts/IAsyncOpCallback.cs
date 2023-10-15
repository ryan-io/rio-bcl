using System;
using System.Threading;

namespace BCL {
	/// <summary>
	///  Abstracts the context of an async operation callback.
	///  See <see cref="AsyncOpCallback{T}"/> for a concrete implementation.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IAsyncOpCallback<T> {
		Action<T, CancellationToken> Context { get; set; }
		void                         Alert(T ctx, CancellationToken token);
	}
}