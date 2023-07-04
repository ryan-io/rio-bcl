using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Source.Events;
using StateMachine;
using UnityBCL;
using UnityBCL.Serialization;
using UnityBCL.Serialization.Core;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Procedural {
	/// <summary>
	///     Verifies the scene contains the required components in order to run procedural generation logic
	///     This class will also kickoff the genereation process
	/// </summary>
	[RequireComponent(
		typeof(ProceduralSceneBounds),
		typeof(ProceduralMeshSolver),
		typeof(ProceduralUtility)
	)]
	public class ProceduralController : Singleton<ProceduralController, ProceduralController>,
	                                    IEngineEventListener<EventStateChange<CreationState>>,
	                                    IEngineEventListener<EventStateChange<ProgressState>> {
		[SerializeField] [Title("Flow")] FlowDependency _flowDependency = new();

		readonly List<IValidate> _validations = new();
		CreationFlow             _creationFlow;
		ProgressFlow             _progressFlow;
		Stopwatch                _stopwatch;
		CancellationTokenSource  _tokenSource;

		WalkabilityRule _walkabilityRule;

		UnityLogging Logger;

		public CancellationToken CancellationToken {
			get {
				if (_tokenSource == null)
					ResetToken();
				return _tokenSource.Token;
			}
		}

		public GameObject             Object                       => gameObject;
		public IEnumerable<ICreation> GetFlowComponentsAsCreation  => _flowDependency.GetComponentsAsCreation;
		public float                  GetTimeElapsedInMilliseconds => _stopwatch?.ElapsedMilliseconds ?? 0.0f;

		[field: SerializeField]
		[field: Title("Configuration")]
		[field: Required]
		public ProceduralCoreConfig CoreConfiguration { get; private set; }

		void Awake() {
			InitializeGeneration();
		}

		async void Start() {
			try {
				await BeginGeneration();
			}
			catch (OperationCanceledException) {
				Logger.Warning("Generation has been canceled...");
			}
		}

		void OnEnable() {
			ResetToken();
			this.StartListeningToEvents<EventStateChange<CreationState>>();
			this.StartListeningToEvents<EventStateChange<ProgressState>>();
		}

		void OnDisable() {
			ResetToken(false);
			this.StopListeningToEvents<EventStateChange<CreationState>>();
			this.StopListeningToEvents<EventStateChange<ProgressState>>();
		}

		void OnDestroy() {
			OnDisable();
		}

		public async void OnEventHeard(EventStateChange<CreationState> e) {
			if (e.NewState == CreationState.DidNotGenerate) {
				var serializerSetup   = _flowDependency.FlowComponents.SerializerSetup;
				var mapSolver         = _flowDependency.FlowComponents.ProceduralMapSolver;
				var tilemapSolver     = _flowDependency.FlowComponents.ProceduralTileSolver;
				var pathfindingSolver = _flowDependency.FlowComponents.ProceduralPathfindingSolver;

				if (!serializerSetup || !mapSolver)
					return;

				var job = new AstarSerializer.AstarDeserializationJob(
					serializerSetup.SaveLocation,
					mapSolver.LastSeed,
					mapSolver.LastIteration,
					mapSolver.MonoModel.ProceduralProceduralMapConfig.NameOfMap);

				AstarSerializer.DeserializeAstarGraph(job);
				Logger.Msg("Deserializing Astar graph data", "Procedural Pathfinding Deserialization");

				var groundTilemap         = tilemapSolver.MonoModel.GroundTilemap;
				var boundaryTilemap       = tilemapSolver.MonoModel.BoundaryTilemap;
				var groundTilemapStatus   = groundTilemap.enabled;
				var boundaryTilemapStatus = boundaryTilemap.enabled;
				groundTilemap.enabled   = true;
				boundaryTilemap.enabled = true;

				var gridGraph = AstarPath.active.data.gridGraph;
				new GridGraphRuleRemover().Remove(gridGraph);

				gridGraph.rules.AddRule(_walkabilityRule);

				await GraphScanSolver.ScanGraphAsync(gridGraph, CancellationToken.None, this);
				Logger.Msg("Deserializing of Astar graph data is complete", "Procedural Pathfinding Deserialization");
				var sm = _flowDependency.FlowComponents.ProceduralMapStateMachine.CreationSm;
				groundTilemap.enabled   = groundTilemapStatus;
				boundaryTilemap.enabled = boundaryTilemapStatus;
				new GraphScanFinalize().Finalize(gridGraph, pathfindingSolver.ScanSettings);
				sm.ChangeState(CreationState.Complete);
			}

			if (e.NewState == CreationState.Complete) CoreConfiguration.RuntimeState = RuntimeState.DoNotGenerate;

			await _creationFlow.HandleFlow(e, GetTimeElapsedInMilliseconds, _tokenSource.Token);
		}

		public async void OnEventHeard(EventStateChange<ProgressState> e) {
			await _progressFlow.HandleFlow(e, GetTimeElapsedInMilliseconds, _tokenSource.Token);
		}
		
		public void ClearConsole() {
#if UNITY_EDITOR || UNITY_STANDALONE
			var assembly = Assembly.GetAssembly(typeof(Editor));
			var type     = assembly.GetType("UnityEditor.LogEntries");
			var method   = type.GetMethod("Clear");
			method.Invoke(new object(), null);
#endif
		}

		public void InitializeGeneration() {
			Logger = new UnityLogging(gameObject.name);

#if UNITY_EDITOR || UNITY_STANDALONE
			ClearConsole();
#endif
			var cellSize = _flowDependency.FlowComponents.ProceduralMapSolver.MonoModel.ProceduralProceduralMapConfig
			                              .CellSize;
			var tileModel = _flowDependency.FlowComponents.ProceduralTileSolver.MonoModel;
			_stopwatch = Stopwatch.StartNew();
			_validations.AddRange(GetComponents<IValidate>());
			_progressFlow    = new ProgressFlow(_flowDependency);
			_walkabilityRule = new WalkabilityRule(tileModel.BoundaryTilemap, tileModel.GroundTilemap, cellSize);
			this.StartListeningToEvents<EventStateChange<CreationState>>();
			this.StartListeningToEvents<EventStateChange<ProgressState>>();
		}

		public async UniTask BeginGeneration() {
			var exitHandler = new ProceduralExitHandler();

			var statements = new Func<bool>[] {
				() => _flowDependency == null,
				() => _flowDependency.GetComponents.IsEmptyOrNull(),
				() => _flowDependency.GetComponents.Any(c => c == null)
			};

			exitHandler.DetermineQuit(statements);

			foreach (var component in _validations)
				component.ValidateShouldQuit();

			_creationFlow = new CreationFlow(_flowDependency);
			_progressFlow = new ProgressFlow(_flowDependency);
			await UniTask.DelayFrame(CoreConfiguration.FramesToDelayBeforeGeneration,
				cancellationToken: CancellationToken);
			_flowDependency.FlowComponents.ProceduralMapStateMachine.CreationSm.ChangeState(CreationState.Cleaning);
		}

		public void Cancel() => _tokenSource?.Cancel();

		public void ResetToken(bool createToken = true) {
			_tokenSource?.Cancel();
			_tokenSource?.Dispose();
			_tokenSource = new CancellationTokenSource();
		}

		static void QuitGame() {
#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
			return;
#endif
			Application.Quit();
		}

		public UniTask Init(CancellationToken token) => new();

		public UniTask Enable(CancellationToken token) => new();

		public UniTask Begin(CancellationToken token) => new();

		public UniTask End(CancellationToken token) => new();

		public UniTask Dispose(CancellationToken token) {
			this.StopListeningToEvents<EventStateChange<CreationState>>();
			this.StopListeningToEvents<EventStateChange<ProgressState>>();
			return new UniTask();
		}

#if UNITY_EDITOR
		bool ShouldFix => _flowDependency != null &&
		                  _flowDependency.FlowComponents.GetPropertyValues<object>().Any(x => x == null);

		[Button]
		[ShowIf("@ShouldFix")]
		void Fix() {
			_flowDependency.FlowComponents.ProceduralMapSolver  = gameObject.FixComponent<ProceduralMapSolver>();
			_flowDependency.FlowComponents.ProceduralMeshSolver = gameObject.FixComponent<ProceduralMeshSolver>();
			_flowDependency.FlowComponents.ProceduralTileSolver = gameObject.FixComponent<ProceduralTileSolver>();
			_flowDependency.FlowComponents.ProceduralPathfindingSolver =
				gameObject.FixComponent<ProceduralPathfindingSolver>();
			_flowDependency.FlowComponents.ProceduralMapStateMachine =
				gameObject.FixComponent<ProceduralMapStateMachine>();
			_flowDependency.FlowComponents.Events = gameObject.FixComponent<ProceduralGenerationEvents>();
			_flowDependency.FlowComponents.ProceduralUtilityCreation =
				gameObject.FixComponent<ProceduralUtility>();
			_flowDependency.FlowComponents.ProceduralScaler     = gameObject.FixComponent<ProceduralScaler>();
			_flowDependency.FlowComponents.ProceduralController = gameObject.FixComponent<ProceduralController>();
			_flowDependency.FlowComponents.SerializerSetup      = gameObject.FixComponent<SerializerSetup>();
		}

#endif
	}
}