using BCL;
using UnityEngine;

namespace UnityBCL {
	public interface ISchedulerSingleton : ISchedule {
		GameObject GlobalGameObject { get; }
		void       RemoveAt(int index, Schedule schedule);
	}
}