// UnityBCL

using System;
using UnityEngine;

namespace UnityBCL {
	public struct EventStateChange<T> where T : struct, IComparable, IConvertible, IFormattable {
		public GameObject      Target;
		public StateMachine<T> TargetStateMachine;
		public T               NewState;
		public T               PreviousState;

		public EventStateChange(StateMachine<T> stateMachine) {
			Target             = stateMachine.Owner;
			TargetStateMachine = stateMachine;
			NewState           = stateMachine.CurrentState;
			PreviousState      = stateMachine.PreviousState;
		}
	}
}