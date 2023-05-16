using OmniBCL.Core;

namespace OmniBCL.Scheduling;

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

	void Update() => Tick(Scheduling.Schedule.Normal);

	void LateUpdate() => Tick(Scheduling.Schedule.Late);

	void FixedUpdate() => Tick(Scheduling.Schedule.Fixed);

	void Awake() => name += " Global";

	readonly ScheduleCollection _schedules = new() {
		{ Scheduling.Schedule.Normal, new HashSet<Action>() },
		{ Scheduling.Schedule.Fixed, new HashSet<Action>() },
		{ Scheduling.Schedule.Late, new HashSet<Action>() }
	};
}