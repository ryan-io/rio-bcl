using System;

namespace BCL {
	/// <summary>
	///  Abstraction for scheduling actions. Used in Unity to schedule actions on the main thread.
	/// </summary>
	public interface ISchedule {
		void Schedule(Action methodToSchedule, Schedule schedule);
		void Unschedule(Action methodToUnschedule, Schedule schedule);
		void Clear(Schedule schedule);
		void ClearAll();
	}
}