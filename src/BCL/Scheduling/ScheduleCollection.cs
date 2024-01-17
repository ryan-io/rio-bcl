using System;
using System.Collections.Generic;

namespace RIO.BCL {
	[Serializable]
	public class ScheduleCollection : Dictionary<Schedule, HashSet<Action>> {
	}
}