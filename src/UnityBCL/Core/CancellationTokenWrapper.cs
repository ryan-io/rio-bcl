using System;
using System.Linq;
using System.Threading;
using BCL;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnityBCL {
	public class CancellationTokenWrapper : IDisposable {
		/// <summary>
		/// This token will be lazily instantiated when a call to GetTokenWithOnDestroy or GetTokenOnDestroyOnly
		/// is invoked.
		/// </summary>
		CancellationToken MonoToken { get; set; } = CancellationToken.None;

		/// <summary>
		/// Primary method for retrieving new CancellationTokens
		/// </summary>
		/// <returns>New CancellationToken</returns>
		public CancellationToken GetToken() {
			HandleDisposal();
			return _cancellationTokenSource.Token;
		}

		/// <summary>
		/// Returns a new CancellationToken relative to a monobehaviors OnDestroy message & the internal token
		/// relative to CancellationTokenWrapper instance. 
		/// </summary>
		/// <param name="monoBehaviour">The monobehavior to create the new token from</param>
		/// <returns>A token from a new LinkedCancellationTokenSource</returns>
		public CancellationToken GetTokenWithOnDestroy(Component monoBehaviour) {
			HandleDisposal();
			ValidateMonoToken(monoBehaviour);
			_linkedCancellationTokenSource =
				CancellationTokenSource.CreateLinkedTokenSource(MonoToken, _cancellationTokenSource.Token);
			return _linkedCancellationTokenSource.Token;
		}

		void ValidateMonoToken(UnityEngine.Component monoBehaviour) {
			if (MonoToken == CancellationToken.None)
				MonoToken = monoBehaviour.GetCancellationTokenOnDestroy();
		}

		/// <summary>
		/// Returns a new CancellationToken ONLY taking into account an external monobehavior
		/// </summary>
		/// <param name="monoBehaviour">The monobehavior context to create a new token from.</param>
		/// <returns>A CancellationToken from Monobehavior.GetCancellationTokenOnDestroy</returns>
		public CancellationToken GetTokenOnDestroyOnly(MonoBehaviour monoBehaviour) {
			return GetNewLinkedToken(monoBehaviour.GetCancellationTokenOnDestroy());
		}

		/// <summary>
		/// Creates a new LinkedCancellationTokenSource and returns a new CancellationToken relative to this linked
		/// source. This linked source also takes into account the internal CancellationToken.
		/// </summary>
		/// <param name="tokens">Any number of external tokens,</param>
		/// <returns>A new linked CancellationToken source from params tokens & the internal instance source.</returns>
		public CancellationToken GetNewLinkedToken(params CancellationToken[] tokens) {
			HandleDisposal();
			var appendedTokens = tokens.Append(_cancellationTokenSource.Token).ToArray();
			_linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(appendedTokens);
			return _linkedCancellationTokenSource.Token;
		}

		/// <summary>
		/// Cancels both the internal & linked CancellationTokenSources
		/// </summary>
		public void Cancel() {
			try {
				_cancellationTokenSource.Cancel();
				_linkedCancellationTokenSource.Cancel();
			}
			catch (ObjectDisposedException) {
				_logging.Log(LogLevel.Normal, "CancellationTokenWrapper has been disposed of.");
			}
		}

		/// <summary>
		/// This needs to be called explicitly by ALL consumers of this class.
		/// </summary>
		public void Dispose() {
			_cancellationTokenSource.Dispose();
			_linkedCancellationTokenSource.Dispose();
		}

		void HandleDisposal() {
			_cancellationTokenSource       = ResetSource(_cancellationTokenSource);
			_linkedCancellationTokenSource = ResetSource(_linkedCancellationTokenSource);
		}

		CancellationTokenSource ResetSource(CancellationTokenSource source) {
			if (source.IsCancellationRequested) {
				source.Dispose();
				return new CancellationTokenSource();
			}

			return source;
		}

		public CancellationTokenWrapper() {
			_cancellationTokenSource       = new CancellationTokenSource();
			_linkedCancellationTokenSource = new CancellationTokenSource();
			_logging                       = new UnityLogging(this);
		}

		CancellationTokenSource _cancellationTokenSource;
		CancellationTokenSource _linkedCancellationTokenSource;
		readonly ILogging       _logging;
	}
}