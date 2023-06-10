using Source;
using UnityBCL;

namespace Procedural {
	public class EventProxy : IEvent {
		public void TriggerEvent<TEvent>(TEvent newEvent) where TEvent : struct {
			Event.TriggerEvent(newEvent);
		}
	}
}