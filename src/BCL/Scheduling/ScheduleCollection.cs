using System;
using System.Collections.Generic;

namespace BCL.Scheduling {
	[Serializable]
	public class ScheduleCollection : Dictionary<Schedule, HashSet<Action>> {
	}
}