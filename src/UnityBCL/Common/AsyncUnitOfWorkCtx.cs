using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UnityBCL {
	public abstract class AsyncUnitOfWorkCtx<T> : AsyncUnitOfWork<T> where T : struct {
		public AsyncUnitOfWorkCtx(Func<T, CancellationToken, UniTask> context) => Context = context;

		public Func<T, CancellationToken, UniTask> Context { get; }


		protected override async UniTask TaskLogic(T args, CancellationToken token) {
			await Context.Invoke(args, token);
		}
	}

	public abstract class AsyncUnitOfWorkCtx : AsyncUnitOfWork {
		public AsyncUnitOfWorkCtx(Func<CancellationToken, UniTask> context) => Context = context;

		public Func<CancellationToken, UniTask> Context { get; }

		protected override async UniTask TaskLogic(CancellationToken token) {
			await Context.Invoke(token);
		}
	}
}