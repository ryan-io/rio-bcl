namespace OmniBCL.Scheduling; 

[Serializable]
internal class ScheduleCollection : Dictionary<Schedule, HashSet<Action>> { }