using System;

namespace RIO.BCL {
	/// <summary>
	///     Use this attribute if you know a class will need to be schedule at runtime.
	///     Beware: this attribute assumes you want to schedule the provided action immediately.
	///     The provided action will be schedule after the 2nd frame of your game completes.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class ScheduleAttribute : Attribute {
		public ScheduleAttribute(Action actionToSchedule, Schedule schedule) {
			// var component = SchedulerComponent.Global;
			// if (component == null)
			// 	throw new NullReferenceException(
			// 		"Could not find a singleton instance of the SchedulerComponent.");
			//
			// component.Schedule(actionToSchedule, schedule);
			// var assembly            = GetType().Assembly;
			// var attributes          = assembly.GetCustomAttributes(typeof(ScheduleAttribute), false);
			// var castedAttributeType = (ScheduleAttribute)attributes[0];
		}
	}
}