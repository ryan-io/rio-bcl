using System.Threading;
using Cysharp.Threading.Tasks;

namespace UnityBCL {
	public class PooledObjectStateChangeUnitOfWork : AsyncUnitOfWork<Arguments.PooledObjectStateChangeArgs> {
		protected override async UniTask
			TaskLogic(Arguments.PooledObjectStateChangeArgs? args, CancellationToken token) {
			if (args == null)
				return;

			await args.Duration.SecAsTask(token);
			if (args.Criteria != null) {
				if (args.Criteria.Invoke(args.WithCriteria))
					args.ObjectToDisable.SetActive(args.ActivationState);
			}
			else {
				args.ObjectToDisable.SetActive(args.ActivationState);
			}
		}
	}
}