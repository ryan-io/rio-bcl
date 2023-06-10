using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UnityBCL {
	public class UniTaskExtensionMethods {
		public static async UniTask CreateDelay(float time, CancellationTokenSource cancelSource)
			=> await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: cancelSource.Token);
	}
}