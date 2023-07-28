using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UnityBCL {
	public class FloatUnitOfWork : AsyncUnitOfWorkCtx<Arguments.FloatArgs> {
		public FloatUnitOfWork(Func<Arguments.FloatArgs, CancellationToken, UniTask> context) : base(context) {
		}
	}
}