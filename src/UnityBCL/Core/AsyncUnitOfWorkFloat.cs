// UnityBCL

using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UnityBCL {
	public class AsyncUnitOfWorkFloat : CtxAsyncUnitOfWork<Arguments.FloatArgs> {
		public AsyncUnitOfWorkFloat(Func<Arguments.FloatArgs, CancellationToken, UniTask> context) : base(context) {
		}

		protected override async UniTask TaskLogic(Arguments.FloatArgs args, CancellationToken token) {
			await Context.Invoke(args, token);
		}
	}
}