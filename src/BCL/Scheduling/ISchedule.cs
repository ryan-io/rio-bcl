﻿using System;

namespace BCL {
	public interface ISchedule {
		void Schedule(Action action, Schedule schedule);
		void Unschedule(Action action, Schedule schedule);
		void Clear(Schedule schedule);
		void ClearAll();
	}
}