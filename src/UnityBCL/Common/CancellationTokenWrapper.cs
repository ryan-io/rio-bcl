using System;
using System.Linq;
using System.Threading;
using RIO.BCL;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnityBCL {
	public class CancellationTokenWrapper : IDisposable {
		/// <summary>
		/// Queries both the internal cancellation token and the linked cancellation token for 'IsCancellationRequested'
		/// </summary>
		public bool IsCancellationRequested => _linkedCancellationTokenSource.IsCancellationRequested ||
		                                     _cancellationTokenSource.IsCancellationRequested;
		
		readonly ILogging _logging;

		CancellationTokenSource _cancellationTokenSource;
		CancellationTokenSource _linkedCancellationTokenSource;

		public CancellationTokenWrapper() {
			_cancellationTokenSource       = new CancellationTokenSource();
			_linkedCancellationTokenSource = new CancellationTokenSource();
			_logging                       = new UnityLogging(this);
		}

		/// <summary>
		///     This token will be lazily instantiated when a call to GetTokenWithOnDestroy or GetTokenOnDestroyOnly
		///     is invoked.
		/// </summary>
		CancellationToken MonoToken { get; set; } = CancellationToken.None;

		/// <summary>
		///     This needs to be called explicitly by ALL consumers of this class.
		/// </summary>
		public void Dispose() {
			_cancellationTokenSource.Dispose();
			_linkedCancellationTokenSource.Dispose();
			_cancellationTokenSource       = new CancellationTokenSource();
			_linkedCancellationTokenSource = new CancellationTokenSource();
		}

		/// <summary>
		///     Primary method for retrieving new CancellationTokens
		/// </summary>
		/// <returns>New CancellationToken</returns>
		public CancellationToken GetToken() {
			Dispose();
			return _cancellationTokenSource.Token;
		}

		/// <summary>
		///     Returns a new CancellationToken relative to a monobehaviors OnDestroy message & the internal token
		///     relative to CancellationTokenWrapper instance.
		/// </summary>
		/// <param name="monoBehaviour">The monobehavior to create the new token from</param>
		/// <returns>A token from a new LinkedCancellationTokenSource</returns>
		public CancellationToken GetTokenWithOnDestroy(Component monoBehaviour) {
			Dispose();
			ValidateMonoToken(monoBehaviour);
			_linkedCancellationTokenSource =
				CancellationTokenSource.CreateLinkedTokenSource(MonoToken, _cancellationTokenSource.Token);
			return _linkedCancellationTokenSource.Token;
		}

		void ValidateMonoToken(Component monoBehaviour) {
			if (MonoToken == CancellationToken.None)
				MonoToken = monoBehaviour.GetCancellationTokenOnDestroy();
		}

		/// <summary>
		///     Returns a new CancellationToken ONLY taking into account an external monobehavior
		/// </summary>
		/// <param name="monoBehaviour">The monobehavior context to create a new token from.</param>
		/// <returns>A CancellationToken from Monobehavior.GetCancellationTokenOnDestroy</returns>
		public CancellationToken GetTokenOnDestroyOnly(MonoBehaviour monoBehaviour)
			=> GetNewLinkedToken(monoBehaviour.GetCancellationTokenOnDestroy());

		/// <summary>
		///     Creates a new LinkedCancellationTokenSource and returns a new CancellationToken relative to this linked
		///     source. This linked source also takes into account the internal CancellationToken.
		/// </summary>
		/// <param name="tokens">Any number of external tokens.</param>
		/// <returns>A new linked CancellationToken source from params tokens & the internal instance source.</returns>
		public CancellationToken GetNewLinkedToken(params CancellationToken[] tokens) {
			Dispose();
			var appendedTokens = tokens.Append(_cancellationTokenSource.Token).ToArray();
			_linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(appendedTokens);
			return _linkedCancellationTokenSource.Token;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="monoBehaviour">The monobehavior context to create a new token from.</param>
		/// <param name="tokens">Any number of external tokens</param>
		/// <returns>A new linked CancellationToken source from params tokens & the internal instance source.</returns>
		public CancellationToken GetNewLinkedTokenWithOnDestroy(MonoBehaviour monoBehaviour, params CancellationToken[] tokens) {
			Dispose();
			var appendedTokens = tokens
			                    .Append( _cancellationTokenSource.Token)
			                    .Append(monoBehaviour.GetCancellationTokenOnDestroy())
			                    .ToArray();
			
			_linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(appendedTokens);
			return _linkedCancellationTokenSource.Token;
		}

		/// <summary>
		///     Cancels both the internal & linked CancellationTokenSources
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
	}
}