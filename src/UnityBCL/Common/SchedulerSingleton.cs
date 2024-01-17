using System;
using System.Collections.Generic;
using System.Linq;
using RIO.BCL;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UnityBCL {
	public class SchedulerSingleton : Singleton<SchedulerSingleton, ISchedulerSingleton>, ISchedulerSingleton {
		ILogging? Logger { get; set; }

		bool RunNormalUpdateInDebug => _runNormalUpdateInDebug == Toggle.Yes;
		bool RunLateUpdateInDebug   => _runLateUpdateInDebug   == Toggle.Yes;
		bool RunFixedUpdateInDebug  => _runFixedUpdateInDebug  == Toggle.Yes;

		void Update() {
			if (_updateActions.IsEmptyOrNull()) return;
			if (RunNormalUpdateInDebug)
				NormalUpdateInDebugMode();
			else
				InvokeActions(_updateActions);

			_events.OnUpdate.Invoke();
		}

		void FixedUpdate() {
			if (_fixedUpdateActions.IsEmptyOrNull()) return;
			if (RunFixedUpdateInDebug)
				FixedUpdateInDebugMode();
			else
				InvokeActions(_fixedUpdateActions);
			_events.OnFixedUpdate.Invoke();
		}

		void LateUpdate() {
			if (_lateUpdateActions.IsEmptyOrNull()) return;
			if (RunLateUpdateInDebug)
				LateUpdateInDebugMode();
			else
				InvokeActions(_lateUpdateActions);

			_events.OnLateUpdate.Invoke();
		}

		public GameObject GlobalGameObject => gameObject;

		public void Clear(Schedule schedule) {
			switch (schedule) {
				case RIO.BCL.Schedule.Normal:
					_updateActions.Clear();
					break;
				case RIO.BCL.Schedule.Late:
					_lateUpdateActions.Clear();
					break;
				case RIO.BCL.Schedule.Fixed:
					_fixedUpdateActions.Clear();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(schedule), schedule, null);
			}
		}

		public void ClearAll() {
			_updateActions.Clear();
			_lateUpdateActions.Clear();
			_fixedUpdateActions.Clear();
		}

		public void Schedule(Action action, Schedule schedule) {
			var actions = GetArray(schedule);

			if (actions.Contains(action))
				return;

			actions.Add(action);
		}

		public void Unschedule(Action action, Schedule schedule) {
			var actions = GetArray(schedule);

			if (actions!.Contains(action))
				return;

			actions.Remove(action);
		}

		public void RemoveAt(int index, Schedule schedule) {
			var actions = GetArray(schedule);

			if (actions.IsEmptyOrNull())
				return;

			actions[index] = null!;
		}

		List<Action> GetArray(Schedule schedule) {
			switch (schedule) {
				case RIO.BCL.Schedule.Normal:
					return _updateActions;
				case RIO.BCL.Schedule.Late:
					return _lateUpdateActions;
				case RIO.BCL.Schedule.Fixed:
					return _fixedUpdateActions;
				default:
					Logger?.Log(LogLevel.Warning, IncorrectSchedule);
					return Array.Empty<Action>().ToList();
			}
		}

		void NormalUpdateInDebugMode() {
			try {
				InvokeActions(_updateActions);
			}
			catch (Exception e) {
				Logger?.Log($"An error was caught: {e.Message}");
			}
		}

		void LateUpdateInDebugMode() {
			try {
				InvokeActions(_lateUpdateActions);
			}
			catch (Exception e) {
				Logger?.Log(LogLevel.Error, $"An error was caught: {e.Message}");
			}
		}

		static void InvokeActions(IEnumerable<Action> actions) {
			foreach (var action in actions)
				action?.Invoke();
		}


		void FixedUpdateInDebugMode() {
			try {
				InvokeActions(_fixedUpdateActions);
			}
			catch (Exception e) {
				Logger?.Log(LogLevel.Error, $"An error was caught: {e.Message}");
			}
		}

#region Plumbing

		void Awake() {
			_updateActions      = new List<Action>();
			_fixedUpdateActions = new List<Action>();
			_lateUpdateActions  = new List<Action>();
			Logger              = new UnityLogging(gameObject);
		}

		List<Action> _updateActions      = null!;
		List<Action> _lateUpdateActions  = null!;
		List<Action> _fixedUpdateActions = null!;

		const string IncorrectSchedule = "An update schedule was not correctly defined when trying to add action.";

		[SerializeField] [Title("Monobehavior Event Hooks")]
		MonoSchedulerEvents _events = null!;

		[Title("Debugging")]
		[SerializeField]
		[EnumToggleButtons]
		[BoxGroup("0", false)]
		[FoldoutGroup("0/RunAsDebug", false)]
		Toggle _runNormalUpdateInDebug = Toggle.No;

		[SerializeField] [EnumToggleButtons] [BoxGroup("0", false)] [FoldoutGroup("0/RunAsDebug", false)]
		Toggle _runLateUpdateInDebug = Toggle.No;

		[SerializeField] [EnumToggleButtons] [BoxGroup("0", false)] [FoldoutGroup("0/RunAsDebug", false)]
		Toggle _runFixedUpdateInDebug = Toggle.No;

#endregion

#if UNITY_EDITOR || UNITY_STANDALONE
		[ShowInInspector]
		[ReadOnly]
		List<Action> UpdateActionsInspector => (Application.isPlaying ? _updateActions : null)!;

		[ShowInInspector]
		[ReadOnly]
		List<Action> FixedUpdateActionsInspector => (Application.isPlaying ? _fixedUpdateActions : null)!;

		[ShowInInspector]
		[ReadOnly]
		List<Action> LateUpdateActionsInspector => (Application.isPlaying ? _lateUpdateActions : null)!;
#endif
	}
}