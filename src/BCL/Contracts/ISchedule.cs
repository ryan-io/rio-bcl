using System;

namespace BCL {
	public interface ISchedule {
		void Schedule(Action methodToSchedule, Schedule schedule);
		void Unschedule(Action methodToUnschedule, Schedule schedule);
		void Clear(Schedule schedule);
		void ClearAll();
	}
}