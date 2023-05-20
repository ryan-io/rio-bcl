using System;
using System.Collections.Generic;
using BCL.Scheduling;

namespace UnityBCL.Core {
	public class SchedulerComponent : Singleton<SchedulerComponent, ISchedule>, ISchedule {
		public void Clear(Schedule schedule) => _schedules[schedule].Clear();

		public void ClearAll() {
			foreach (var schedule in _schedules.Values)
				schedule.Clear();
		}

		public void Schedule(Action action, Schedule schedule) {
			if (GetScheduleActions(schedule).Contains(action))
				return;

			GetScheduleActions(schedule).Add(action);
		}

		public void Unschedule(Action action, Schedule schedule) {
			if (!GetScheduleActions(schedule).Contains(action))
				return;

			GetScheduleActions(schedule).Remove(action);
		}

		HashSet<Action> GetScheduleActions(Schedule schedule) => _schedules[schedule];

		void Tick(Schedule schedule) {
			var scheduledActions = GetScheduleActions(schedule);

			if (scheduledActions.Count < 1)
				return;

			foreach (var action in scheduledActions)
				action.Invoke();
		}

		void Update() => Tick(BCL.Scheduling.Schedule.Normal);

		void LateUpdate() => Tick(BCL.Scheduling.Schedule.Late);

		void FixedUpdate() => Tick(BCL.Scheduling.Schedule.Fixed);

		void Awake() => name += " Global";

		readonly ScheduleCollection _schedules = new() {
			{ BCL.Scheduling.Schedule.Normal, new HashSet<Action>() },
			{ BCL.Scheduling.Schedule.Fixed, new HashSet<Action>() },
			{ BCL.Scheduling.Schedule.Late, new HashSet<Action>() }
		};
	}
}