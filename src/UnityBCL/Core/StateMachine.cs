using System;
using UnityEngine;

namespace UnityBCL {
	/// <summary>
	///     Public interface for the state machine.
	///     This is used by the StateMachineProcessor.
	/// </summary>
	public interface IStateMachine {
	}

	/// <summary>
	///     StateMachine manager, designed with simplicity in mind (as simple as a state machine can be anyway).
	///     To use it, you need an enum. For example : public enum CharacterConditions { Normal, ControlledMovement, Frozen,
	///     Paused, Dead }
	///     Declare it like so : public StateMachine
	///     <CharacterConditions>
	///         ConditionStateMachine;
	///         Initialize it like that : ConditionStateMachine = new StateMachine
	///         <CharacterConditions>
	///             ();
	///             Then from anywhere, all you need to do is update its state when needed, like that for example :
	///             ConditionStateMachine.ChangeState(CharacterConditions.Dead);
	///             The state machine will store for you its current and previous state, accessible at all times, and will also
	///             optionnally trigger events on enter/exit of these states.
	///             You can go further by using a StateMachineProcessor class, to trigger more events (see the list below).
	/// </summary>
	public class StateMachine<T> : IStateMachine where T : struct, IComparable, IConvertible, IFormattable {
		public delegate void OnStateChangeDelegate();

		readonly IEvent _eventHandler;
		readonly bool   _triggerEvents;

		/// <summary>
		///     Creates a new StateMachine, with a targetName (used for events, usually use GetInstanceID()), and whether you want
		///     to use events with it or not
		/// </summary>
		/// <param name="target">GameObject target</param>
		/// <param name="eventHandler">Event manager proxy</param>
		/// <param name="triggerEvents">If set to <c>true</c> trigger events.</param>
		public StateMachine(GameObject target, IEvent eventHandler, bool triggerEvents) {
			Target         = target;
			_triggerEvents = triggerEvents;
			_eventHandler  = eventHandler;
		}

		/// If you set TriggerEvents to true, the state machine will trigger events when entering and exiting a state. 
		/// Additionnally, if you also use a StateMachineProcessor, it'll trigger events for the current state on FixedUpdate, LateUpdate, but also
		/// on Update (separated in EarlyUpdate, Update and EndOfUpdate, triggered in this order at Update()
		/// To listen to these events, from any class, in its Start() method (or wherever you prefer), use MMEventManager.StartListening(gameObject.GetInstanceID().ToString()+"XXXEnter",OnXXXEnter);
		/// where XXX is the name of the state you're listening to, and OnXXXEnter is the method you want to call when that event is triggered.
		/// MMEventManager.StartListening(gameObject.GetInstanceID().ToString()+"CrouchingEarlyUpdate",OnCrouchingEarlyUpdate); for example will listen to the Early Update event of the Crouching state, and 
		/// will trigger the OnCrouchingEarlyUpdate() method.

		public GameObject Target { get; }

		public T CurrentState { get; protected set; }

		public T PreviousState { get; protected set; }

		/// an event you can listen to to listen locally to changes on that state machine
		/// to listen to them, from any class : 
		/// void OnEnable()
		/// {
		/// yourReferenceToTheStateMachine.OnStateChange += OnStateChange;
		/// }
		/// void OnDisable()
		/// {
		/// yourReferenceToTheStateMachine.OnStateChange -= OnStateChange;
		/// }
		/// void OnStateChange()
		/// {
		/// // Do something
		/// }
		public event OnStateChangeDelegate? OnStateChange;

		public void DeleteSubscribers() => OnStateChange = null;

		/// <summary>
		///     Changes the current movement state to the one specified in the parameters, and triggers exit and enter events if
		///     needed
		/// </summary>
		/// <param name="newState">New state.</param>
		public virtual void ChangeState(T newState) {
			// if the "new state" is the current one, we do nothing and exit
			if (newState.Equals(CurrentState)) return;

			// we store our previous character movement state
			PreviousState = CurrentState;
			CurrentState  = newState;

			OnStateChange?.Invoke();

			if (_triggerEvents) _eventHandler.TriggerEvent(new EventStateChange<T>(this));
		}

		/// <summary>
		///     Returns the character to the state it was in before its current state
		/// </summary>
		public virtual void RestorePreviousState() {
			// we restore our previous state
			CurrentState = PreviousState;

			OnStateChange?.Invoke();

			if (_triggerEvents) _eventHandler.TriggerEvent(new EventStateChange<T>(this));
		}
	}
}