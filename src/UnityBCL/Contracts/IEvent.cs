// UnityBCL

namespace UnityBCL {
	public interface IEvent {
		void TriggerEvent<TEvent>(TEvent newEvent) where TEvent : struct;
	}
}