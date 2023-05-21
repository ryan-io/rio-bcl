using System;
using System.Threading;
using BCL;
using Cysharp.Threading.Tasks;

namespace UnityBCL {
	public abstract class AsyncUnitOfWork<TArgs> : IDisposable {
		protected CancellationTokenWrapper Cancellation { get; }
		
		protected bool IsAwaiting { get; set; }

		/// <summary>
		/// This is a static call that functions that same as an instanced call to Run().
		/// This call is independent of an internal (instance) CancellationTokenSource.
		/// It is preferred to inherit from this class and to run the logic with an internal token source.
		/// This call allows MANY invocations with ANY cancellation token.
		/// An instanced version of this class will prevent a consumer from invoking their  asynchronous
		/// logic twice.
		/// </summary>
		/// <param name="args">Any necessary data to allow the logic to process</param>
		/// <param name="externalToken">External token to be used in a linked token source</param>
		public async UniTaskVoid Fire(TArgs args, CancellationToken externalToken) {
			if (IsAwaiting)
				return;

			var token = Cancellation.GetNewLinkedToken(externalToken);

			try {
				IsAwaiting = true;
				await TaskLogic(args, token);
			}
			catch (Exception e) {
				_logging.Log(LogLevel.Error, e.Message);
				Cancellation.Cancel();
			}
			finally {
				IsAwaiting = false;
				Cancellation.Dispose();
			}
		}

		public void Dispose() {
			Cancellation.Cancel();
			Cancellation.Dispose();
		}

		protected abstract UniTask TaskLogic(TArgs args, CancellationToken token);

		protected AsyncUnitOfWork() {
			Cancellation = new CancellationTokenWrapper();
			_logging     = new UnityLogging(this);
		}

		readonly ILogging _logging;
	}
}