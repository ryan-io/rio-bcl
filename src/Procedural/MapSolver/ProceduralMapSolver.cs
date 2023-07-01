using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BCL;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Unity.Serialization;
using Unity.Serialization.Serialization;
using UnityBCL;
using UnityEngine;

namespace Procedural {
	[RequireComponent(typeof(ProceduralController))]
	public class ProceduralMapSolver : Singleton<ProceduralMapSolver, IProceduralMap>,
	                                   IProceduralMap, IValidate, ICreation, IProgress {
		[SerializeField] [ReadOnly] int _lastIteration;

		[SerializeField] [ReadOnly] [Title("Current Serialization")]
		string _lastSeed;

		BorderAndBoundsSolver   _borderAndBoundsSolver;
		CancellationTokenSource _editorTokenSource;
		ProceduralExitHandler   _exitHandler;

		FillMapSolver _fillMapSolver;

		RegionRemovalSolver _regionRemovalSolver;
		RoomSolver          _roomSolver;
		SmoothMapSolver     _smoothMapSolver;
		UnityLogging        Logger        { get; } = new();
		public string       LastSeed      => _lastSeed;
		public int          LastIteration => _lastIteration;

		public async UniTask Init(CancellationToken token) {
			await UniTask.Yield();
			var config = MonoModel.ProceduralProceduralMapConfig;

			Model = new ProceduralMapModel {
				Map = new int[config.MapWidth, config.MapHeight],
				BorderMap = new int[
					config.MapWidth  + config.MapBorderSize * 2,
					config.MapHeight + config.MapBorderSize * 2]
			};

			_exitHandler = new ProceduralExitHandler();
		}

		public UniTask Enable(CancellationToken token) => new();

		//EVENTS///////////////////////////////////////////////////////////////////////////////////////////////
		// 	var config = Model.ProceduralProceduralMapConfig;
		// ProceduralMapStateMachine.Global.Register_MapGenerationComplete(Model.OnMapGenerateComplete.Invoke);
		// ProceduralMapStateMachine.Global.Register_MapGenerationComplete(CreateRoomsDirector);
		//
		// ProceduralMapStateMachine.Global.Register_LifeCycleEnd(Model.OnLifeCycleEnd.Invoke);
		// ProceduralMapStateMachine.Global.Register_LifeCycleEnd(DisposeOfCancellationTokenSource);
		//
		// ProceduralMapStateMachine.Global.Register_GenerationComplete(Model.OnGenerationComplete.Invoke);
		// ProceduralMapStateMachine.Global.Register_GenerationComplete(_model.SceneBounds
		//    .OnCompleteInvokeBoundaryEvent);
		//
		// if (config.syncWithPathfinding == Toggle.Yes && _model.PathfindingBridge != null) {
		// 	_model.PathfindingBridge.RegisterToScanComplete(PathfindingSyncComplete);
		// }
		//
		// else {
		// 	ProceduralMapStateMachine.Global.Register_GenerationComplete(PostSyncWithPathfinding);
		// }
		/////////////////////////////////
		public UniTask Begin(CancellationToken token) {
			var config = MonoModel.ProceduralProceduralMapConfig;

			if (config.UseRandomSeed == Toggle.Yes)
				config.Seed = NumberSeeding.CreateRandomSeed();

			if (_lastSeed == config.Seed)
				_lastIteration++;
			else
				_lastIteration = 0;

			_lastSeed       = config.Seed;
			Model.SeedValue = NumberSeeding.GetSeedHash(config.Seed); // hashcode to introduce "randomness"

			var fillMapSolverModel         = new MapSolverModel(config);
			var borderAndBoundsSolverModel = new BorderAndBoundsSolverModel(Model, config);
			_fillMapSolver         = new CellularAutomataFillMapSolver(fillMapSolverModel);
			_smoothMapSolver       = new MarchingSquaresSmoothMapSolver(fillMapSolverModel);
			_borderAndBoundsSolver = new IterativeBorderAndBoundsSolver(fillMapSolverModel, borderAndBoundsSolverModel);
			_regionRemovalSolver   = new FloodRegionRemovalSolver(fillMapSolverModel);

			return new UniTask();
		}

		public UniTask End(CancellationToken token) => new();


		public UniTask Dispose(CancellationToken token) =>
			//await _roomSolver.Dispose();
			new();

		[field: SerializeField]
		[field: HideLabel]
		public ProceduralMapMonobehaviorModel MonoModel { get; private set; }

		public ProceduralMapModel Model { get; private set; }

		public async UniTask Progress_PopulatingMap(CancellationToken token) {
			Model.Map = await _fillMapSolver.Fill(Model.Map, token);
		}

		public async UniTask Progress_SmoothingMap(CancellationToken token)
			=> Model.Map = await _smoothMapSolver.Smooth(Model.Map, token);

		public async UniTask Progress_RemovingRegions(CancellationToken token)
			=> Model.Map = await _regionRemovalSolver.Remove(Model.Map, token);

		public async UniTask Progress_CreatingBoundary(CancellationToken token)
			=> Model.BorderMap = await _borderAndBoundsSolver.Determine(Model.BorderMap, Model.Map, token);

		public async UniTask Progress_CompilingData(CancellationToken token) {
			_roomSolver = new SimpleRoomSolver();
			await _roomSolver.Solve();

			Logger.Test("Serializing tracker data", "Data Serialization");
			var mapStats = new MapStats(_lastSeed, _lastIteration, MonoModel.ProceduralProceduralMapConfig.NameOfMap);
			ProceduralMapStatsHelper.WriteNewSeed(mapStats, GetComponent<SerializerSetup>());
		}

		public UniTask Progress_PreparingAndSettingTiles(CancellationToken token) => new();
		public UniTask Progress_GeneratingMesh(CancellationToken token)           => new();

		public UniTask Progress_CalculatingPathfinding(PathfindingProgressData progressData, CancellationToken token)
			=> new();

		// void OnValidate() {
		// 	if (Application.isPlaying)
		// 		return;
		// 	//
		// 	// if (gameObject.layer != LayerMask.NameToLayer(ProceduralLogging.ObstaclesLayerName))
		// 	// 	gameObject.layer = LayerMask.NameToLayer(ProceduralLogging.ObstaclesLayerName);
		// 	//
		// 	// if (!gameObject.CompareTag(ProceduralLogging.LevelGeneratorTag))
		// 	// 	gameObject.tag = ProceduralLogging.LevelGeneratorTag;
		// }

		void IValidate.ValidateShouldQuit() {
			_exitHandler = new ProceduralExitHandler();

			var statements = new HashSet<Func<bool>> {
				() => MonoModel                               == null,
				() => Model                                   == null,
				() => MonoModel.ProceduralProceduralMapConfig == null
			};

			_exitHandler.DetermineQuit(statements.ToArray());
		}
	}

//------------------------------------------------------------------------------------------------------------->
	public abstract class RoomSolver {
		public abstract UniTask Solve();
		public abstract UniTask Dispose();
	}

	public class SimpleRoomSolver : RoomSolver {
		//CreateMapModel(MonoModel.BoundaryTilemap, MonoModel.GroundTilemap);
		//EventManager.TriggerEvent(_cachedMapModel);
		//CreateRoomsDirector(_cachedMapModel);

		/*
		 * 			var config = Model.ProceduralProceduralMapConfig;

		if (ProceduralMapStateMachine.Global.ShouldNotGenerateAtRuntime) {
			if (Model.RoomDataPreGenerated == null) {
				log.Error(GeneratorMessages.NoRoomDataAssigned);

				return;
			}

			_model.MapHandler = new MapHandler(Model.RoomDataPreGenerated, data, _model.SeedValue);
		}

		else {
			var roomData = _model.MapProcessor.Save(_model.SeedValue.ToString(), config.seed);

			Model.RoomDataGenerated = Model.RoomDataPreGenerated = roomData;
			_model.MapHandler      = new MapHandler(roomData, data, _model.SeedValue);
		}
		 */

		// ProceduralMapStateMachine.Global.SetInProgressStateWithData(
		// 	ProgressState.SyncingPathfinding, _cachedModel);
		// SetStateToComplete();

		public override UniTask Solve() => new();

		public override UniTask Dispose() => new();
		// var config = MonoModel.ProceduralProceduralMapConfig;
		// if (!Application.isPlaying || !config.ShouldNotGenerate)
		// 	MonoModel.RoomDataGenerated = null;
	}
}