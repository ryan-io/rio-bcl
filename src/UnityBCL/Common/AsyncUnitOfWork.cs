using System;
using System.Threading;
using RIO.BCL;
using RIO.BCL.Unmanaged;
using Cysharp.Threading.Tasks;

namespace UnityBCL {
/*		*****************************************************************
 * The in parameters of value types are passed by reference, and that means that the cost of passing an argument is
 * constant and doesn’t depend on the size of the struct. This is a good news.
 *///	*****************************************************************

	/// <summary>
	/// Defines a unit of work. Inheriting classes should implement TaskLogic. This logic should be asynchronous.
	/// TArgs is defined to be any immutable struct that contains relevant references to other objects in your code
	/// base.
	/// </summary>
	/// <typeparam name="TArgs">Used to pass required references to your task logic.</typeparam>
	public abstract class AsyncUnitOfWork<TArgs> : IDisposable where TArgs : struct {
		protected AsyncUnitOfWork() {
			Cancellation = new CancellationTokenWrapper();
			Logging      = new UnityLogging(this);
		}
		
		public bool IsAwaiting { get; protected set; }

		protected CancellationTokenWrapper Cancellation { get; }

		protected bool IsCancellationRequested => Cancellation.IsCancellationRequested;

		protected ILogging Logging { get; }

		/// <summary>
		/// Cancels the 'wrapped; tokens and handle disposing on them.
		/// </summary>
		public void Dispose() {
			Cancellation.Cancel();
			Cancellation.Dispose();
		}

		/// <summary>
		///     This is a static call that functions that same as an instanced call to Run().
		///     This call is independent of an internal (instance) CancellationTokenSource.
		///     It is preferred to inherit from this class and to run the logic with an internal token source.
		///     This call allows MANY invocations with ANY cancellation token.
		///     An instanced version of this class will prevent a consumer from invoking their  asynchronous
		///     logic twice.
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
				Logging.Log(LogLevel.Error, e.Message);
				Cancellation.Cancel();
			}
			finally {
				IsAwaiting = false;
				Cancellation.Dispose();
			}
		}

		public void FireForget(in TArgs args, CancellationToken externalToken) {
			Fire(args, externalToken).Forget();
		}

		public void FireForgetNoArgs(CancellationToken externalToken) {
			Fire(default, externalToken).Forget();
		}

		public async UniTask FireTask(TArgs args, CancellationToken externalToken) {
			if (IsAwaiting)
				return;

			var token = Cancellation.GetNewLinkedToken(externalToken);

			try {
				IsAwaiting = true;
				await TaskLogic(args, token);
			}
			catch (Exception e) {
				Logging.Log(LogLevel.Error, e.Message);
				Cancellation.Cancel();
			}
			finally {
				OnFinally(args);
				IsAwaiting = false;
				Cancellation.Dispose();
			}
		}

		/// <summary>
		/// Internal logic that should be run asynchronously. This is the core method of an asynchronous unit of work.
		/// </summary>
		/// <param name="args">Any pertinent data that should be passed along in order to run TaskLogic</param>
		/// <param name="token">Any external token that should also be taken into account for cancellation.</param>
		/// <returns>A UniTask to await. Invoke should be considered asynchronous</returns>
		protected abstract UniTask TaskLogic(TArgs args, CancellationToken token);
		
		/// <summary>
		/// Override this method and define any synchronous logic that should be invoked in the Finally block of a
		/// Try-Catch asynchronous process.
		/// </summary>
		protected virtual void OnFinally(TArgs args) {}
	}

	
	/// <summary>
	/// See above: AsyncUnitOfWork|TArgs|
	/// This class is virtually identical; this of course is a non-generic version.
	/// </summary>
	public abstract class AsyncUnitOfWork : IDisposable {
		protected AsyncUnitOfWork() {
			Cancellation = new CancellationTokenWrapper();
			Logging      = new UnityLogging(this);
		}

		public bool IsAwaiting { get; protected set; }

		protected CancellationTokenWrapper Cancellation { get; }

		protected ILogging Logging { get; }

		public void Dispose() {
			Cancellation.Cancel();
			Cancellation.Dispose();
		}
		
		/// <summary>
		///     This is a static call that functions that same as an instanced call to Run().
		///     This call is independent of an internal (instance) CancellationTokenSource.
		///     It is preferred to inherit from this class and to run the logic with an internal token source.
		///     This call allows MANY invocations with ANY cancellation token.
		///     An instanced version of this class will prevent a consumer from invoking their  asynchronous
		///     logic twice.
		/// </summary>
		/// <param name="externalToken">External token to be used in a linked token source</param>
		public async UniTaskVoid Fire(CancellationToken externalToken) {
			if (IsAwaiting)
				return;

			var token = Cancellation.GetNewLinkedToken(externalToken);

			try {
				IsAwaiting = true;
				await TaskLogic(token);
			}
			catch (Exception e) {
				Logging.Log(LogLevel.Error, e.Message);
				Cancellation.Cancel();
			}
			finally {
				OnFinally();
				IsAwaiting = false;
				Cancellation.Dispose();
			}
		}

		public void FireForget(CancellationToken token) {
			Fire(token).Forget();
		}

		public async UniTask FireTask(CancellationToken externalToken) {
			if (IsAwaiting)
				return;

			var token = Cancellation.GetNewLinkedToken(externalToken);

			try {
				IsAwaiting = true;
				await TaskLogic(externalToken);
			}
			catch (Exception e) {
				Logging.Log(LogLevel.Error, e.Message);
				Cancellation.Cancel();
			}
			finally {
				IsAwaiting = false;
				Cancellation.Dispose();
			}
		}

		/// <summary>
		/// Internal logic that should be run asynchronously. This is the core method of an asynchronous unit of work.
		/// </summary>
		/// <param name="token">Any external token that should also be taken into account for cancellation.</param>
		/// <returns>A UniTask to await. Invoke should be considered asynchronous</returns>
		protected abstract UniTask TaskLogic(CancellationToken token);
		
		/// <summary>
		/// Override this method and define any synchronous logic that should be invoked in the Finally block of a
		/// Try-Catch asynchronous process.
		/// </summary>
		protected virtual void OnFinally() {}
	}
}