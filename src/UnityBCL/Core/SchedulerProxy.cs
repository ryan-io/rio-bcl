using System;
using BCL;

namespace UnityBCL {


	/// <summary>
	/// This class is intended for use where instances of using a singleton aren't ideal.
	/// Recommended to implement your own class with ISchedule. The base constructor will use
	/// the Unity game object singleton.
	/// </summary>
	public class SchedulerProxy : ISchedule {
		public void Schedule(Action action, Schedule schedule) {
			_scheduler.Schedule(action, schedule);
		}

		public void Unschedule(Action action, Schedule schedule) {
			_scheduler.Unschedule(action, schedule);
		}

		public void Clear(Schedule schedule) {
			_scheduler.Clear(schedule);
		}

		public void ClearAll() {
			_scheduler.ClearAll();
		}

		public SchedulerProxy() {
			_scheduler = SchedulerComponent.Global;
		}

		public SchedulerProxy(ISchedule scheduler) {
			_scheduler = scheduler;
		}

		readonly ISchedule _scheduler;
	}
}