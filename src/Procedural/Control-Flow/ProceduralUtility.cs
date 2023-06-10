using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Source;
using Unity.Mathematics;
using UnityBCL;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace Procedural {
	[Serializable]
	public class ProceduralMapUtilityMonobehaviorModel {
		[field: SerializeField]
		[field: Required]
		public ProceduralMapStateMachine ProceduralMapStateMachine { get; set; }

		[field: SerializeField]
		[field: Required]
		public ProceduralMapSolver ProceduralMapSolver { get; set; }

		[field: SerializeField]
		[field: Required]
		public ProceduralTileSolver ProceduralTileSolver { get; set; }
	}

	public class ProceduralUtility : Singleton<ProceduralUtility, ProceduralUtility>,
	                                 ICreation,
	                                 IEventListener<EventStateChange<ProgressState>>,
	                                 IEventListener<GridSetEvent> {
		[Title("Required Monobehaviors")] [SerializeField] [HideLabel]
		ProceduralMapUtilityMonobehaviorModel _monoModel;

		public static double TotalMemoryAllocated
			=> Math.Round(Profiler.GetTotalAllocatedMemoryLong() / 1000000000f, 2);

		bool ShouldFix => _monoModel != null && _monoModel.GetPropertyValues<object>().Any(x => x == null);

		public UniTask Init(CancellationToken token) {
			this.StartListeningToEvents<GridSetEvent>();
			this.StartListeningToEvents<EventStateChange<ProgressState>>();
			return new UniTask();
		}

		public UniTask Enable(CancellationToken token) => new();

		public UniTask Begin(CancellationToken token) => new();

		public UniTask End(CancellationToken token) {
			this.StopListeningToEvents<GridSetEvent>();
			this.StopListeningToEvents<EventStateChange<ProgressState>>();
			return new UniTask();
		}

		public UniTask Dispose(CancellationToken token) => new();

		public void OnEventHeard(EventStateChange<ProgressState> e) {
			if (e.NewState == ProgressState.Disposing) RemoveLabels();
		}

		public void OnEventHeard(GridSetEvent e) {
			/*
			 * 	MapGridDirector.SetGridOrigin(_tileSceneObjects, _generatorConfig.ConfigDimensions);
			MapGridDirector.SetGridScale(_tileSceneObjects, _generatorConfig.cellSize);
			 */
			GridUtil.SetGridOrigin(e.SceneObjects, e.MapSize);
			GridUtil.SetGridScale(e.SceneObjects, e.Cellsize);
		}

		public static void LogAllocatedMemory() {
			var log = new UnityLogging();
			log.Msg(
				$"Memory usage: {TotalMemoryAllocated} GB"
				, size: 15, italic: true, bold: true, ctx: $"{Strings.ProcGen} Heap Allocation");
		}

		public static float DetermineTotalTime(float milliseconds, out string unit) {
			float totalTime;

			if (milliseconds >= 1000) {
				totalTime = milliseconds / 1000f;
				unit      = "seconds";
			}

			else {
				totalTime = milliseconds;
				unit      = "mSeconds";
			}

			return totalTime;
		}

		[Button(ButtonSizes.Medium)]
		public void RemoveLabels() {
			var objects = GameObject.FindGameObjectsWithTag(TileMapper.Label);
			var length  = objects.Length;

			for (var i = 0; i < length; i++)
				if (Application.isPlaying)
					Destroy(objects[i]);

				else
					DestroyImmediate(objects[i]);
		}

#if UNITY_EDITOR || UNITY_STANDALONE
		void RemoveTiles(ProceduralTileSceneObjects tileSceneObjects, int2 mapSize) {
			var shouldEraseAll = true;


			if (!Application.isPlaying)
				shouldEraseAll = EditorUtility.DisplayDialog(
					"Reset All", "Reset All Procedural Generation?", "Yes",
					"No");

			foreach (var map in tileSceneObjects.TileMapTable) {
				var shouldRemove = true;
				var sm           = _monoModel.ProceduralMapStateMachine;

				if (!shouldEraseAll && sm         != null                    &&
				    sm.ApplicationSm.CurrentState == ApplicationState.Editor && Application.isPlaying)
					shouldRemove = EditorUtility.DisplayDialog(
						"Reset Tiles",
						$"Do you want to remove all tiles on the {map.Key} map?",
						"Yes", "No");


				if (!shouldRemove)
					continue;

				var mapSizeX = mapSize.x;
				var mapSizeY = mapSize.y;

				for (var x = 0; x < mapSizeX; x++) {
					for (var y = 0; y < mapSizeY; y++)
						Utility.SetTileNullAtXY(x, y, map);
				}
			}
		}

#endif

		[Button]
		[ShowIf("@ShouldFix")]
		void Fix() {
			_monoModel.ProceduralMapSolver       = gameObject.FixComponent<ProceduralMapSolver>();
			_monoModel.ProceduralTileSolver      = gameObject.FixComponent<ProceduralTileSolver>();
			_monoModel.ProceduralMapStateMachine = gameObject.FixComponent<ProceduralMapStateMachine>();
		}
	}
}