// UnityBCL

using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UnityBCL {
	public abstract class CtxAsyncUnitOfWork<T> : AsyncUnitOfWork<T> {
		public CtxAsyncUnitOfWork(Func<T, CancellationToken, UniTask> context) => Context = context;

		public Func<T, CancellationToken, UniTask> Context { get; set; }
	}
}