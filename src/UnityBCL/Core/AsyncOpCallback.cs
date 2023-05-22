using System;
using System.Threading;
using BCL;

namespace UnityBCL {
	/// <summary>
	/// Used by AsyncOp classes to provide minimal callback context data to the invokers.
	/// </summary>
	/// <typeparam name="T">Generic type of context to return.</typeparam>
	public class AsyncOpCallback<T> : IAsyncOpCallback<T> {
		public static Action<T, CancellationToken> Empty => default!;
		
		public Action<T, CancellationToken> Context { get; set; } = null!;

		public void Alert(T ctx, CancellationToken token) {
			Context.Invoke(ctx, token);
		}
	}
}