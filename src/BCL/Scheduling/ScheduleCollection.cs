using System;
using System.Collections.Generic;

namespace BCL {
	[Serializable]
	public class ScheduleCollection : Dictionary<Schedule, HashSet<Action>> {
	}
}