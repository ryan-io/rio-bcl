using System;
using Unity.Burst;
using Unity.Jobs;

namespace Procedural {
	[BurstCompile]
	public struct SimpleMapCalculateJob : IJobParallelFor {
		public void Execute(int index) {
			throw new NotImplementedException();
		}
	}
}