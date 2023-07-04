using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Source.Events;
using UnityBCL;
using UnityEngine;
using StateMachine;

namespace Procedural {
	public interface IProceduralMapStateMachine {
	}

	public class ProceduralMapStateMachine : Singleton<ProceduralMapStateMachine, IProceduralMapStateMachine>,
	                                         ICreation, IValidate, IEngineEventListener<CreationState> {
		[SerializeField] [HideLabel] ProceduralMapStateMachineMonobehaviorModel _monoModel;
		public                       StateMachine<ApplicationState>             ApplicationSm { get; private set; }
		public                       StateMachine<RuntimeState>                 RuntimeSm     { get; private set; }
		public                       StateMachine<CreationState>                CreationSm    { get; private set; }
		public                       StateMachine<ProgressState>                ProgressSm    { get; private set; }

		bool ShouldFix => _monoModel != null && _monoModel.GetPropertyValues<object>().Any(x => x == null);

		EngineEventProxy EventProxy { get; set; }

		void Awake() {
			EventProxy = new EngineEventProxy();
			CreateStateMachines();
		}

		void Start() {
			SetStateMachines();
		}

		void OnEnable() {
			RegisterStateMachines();
		}

		public UniTask Init(CancellationToken token) => new();

		public UniTask Enable(CancellationToken token) => new();

		public UniTask Begin(CancellationToken token) => new();

		public UniTask End(CancellationToken token) {
			ApplicationSm.DeleteSubscribers();
			CreationSm.DeleteSubscribers();
			RuntimeSm.DeleteSubscribers();
			ProgressSm.DeleteSubscribers();
			return new UniTask();
		}

		public UniTask Dispose(CancellationToken token) {
			this.StartListeningToEvents();
			return new UniTask();
		}

		public void OnEventHeard(CreationState e) {
		}

		public void ValidateShouldQuit() {
			var exitHandler = new ProceduralExitHandler();

			var statements = new HashSet<Func<bool>> {
				() => _monoModel                      == null,
				() => _monoModel.ProceduralController == null
			};

			exitHandler.DetermineQuit(statements.ToArray());
		}

		public void CreateStateMachines() {
			this.StartListeningToEvents();
			var go = gameObject;
			ApplicationSm = new StateMachine<ApplicationState>(go, true);
			CreationSm    = new StateMachine<CreationState>(go, true);
			ProgressSm    = new StateMachine<ProgressState>(go, true);
			RuntimeSm     = new StateMachine<RuntimeState>(go, true);
		}

		public void RegisterStateMachines() {
			ApplicationSm.OnStateChange +=
				() => ProceduralLogging.LogStateChange(
					typeof(ApplicationState), ApplicationSm.CurrentState,
					_monoModel.ProceduralController.GetTimeElapsedInMilliseconds);
			CreationSm.OnStateChange +=
				() => ProceduralLogging.LogStateChange(typeof(CreationState), CreationSm.CurrentState,
					_monoModel.ProceduralController.GetTimeElapsedInMilliseconds);
			ProgressSm.OnStateChange +=
				() => ProceduralLogging.LogStateChange(typeof(ProgressState), ProgressSm.CurrentState,
					_monoModel.ProceduralController.GetTimeElapsedInMilliseconds);
			RuntimeSm.OnStateChange +=
				() => ProceduralLogging.LogStateChange(typeof(RuntimeState), RuntimeSm.CurrentState,
					_monoModel.ProceduralController.GetTimeElapsedInMilliseconds);
		}

		public void SetStateMachines() {
			var coreConfig = _monoModel.ProceduralController.CoreConfiguration;
			ApplicationSm.ChangeState(coreConfig.ApplicationState);
			RuntimeSm.ChangeState(coreConfig.RuntimeState);
			ProgressSm.ChangeState(ProgressState.Pending);
			CreationSm.ChangeState(CreationState.Pending);
		}

#if UNITY_EDITOR || UNITY_STANDALONE
		public void ForceAllInitialize() {
			CreateStateMachines();
			RegisterStateMachines();
			SetStateMachines();
		}
#endif

		[Button]
		[ShowIf("@ShouldFix")]
		void Fix() {
			_monoModel.ProceduralController = gameObject.FixComponent<ProceduralController>();
		}
	}
}