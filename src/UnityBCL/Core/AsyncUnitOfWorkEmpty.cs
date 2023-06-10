using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UnityBCL {
	public class AsyncUnitOfWorkEmpty : CtxAsyncUnitOfWork<Arguments.EmptyArgs> {
		public AsyncUnitOfWorkEmpty(Func<Arguments.EmptyArgs, CancellationToken, UniTask> context) : base(context) {
		}

		protected override async UniTask TaskLogic(Arguments.EmptyArgs? args, CancellationToken token) {
			args ??= Arguments.EmptyArgs.GetNew();
			await Context.Invoke(args, token);
		}
	}
}