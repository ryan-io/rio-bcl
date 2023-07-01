using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UnityBCL {
	public class EmptyUnitOfWorkCtx : AsyncUnitOfWorkCtx {
		public EmptyUnitOfWorkCtx(Func<CancellationToken, UniTask> context) : base(context) {
		}
	}
}