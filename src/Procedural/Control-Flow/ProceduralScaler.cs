using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Source;
using Unity.Mathematics;
using UnityBCL;
using UnityEngine;

namespace Procedural {
	[Serializable]
	public class ProceduralScalerMonobehaviorModel {
		[field: SerializeField]
		[field: Title("Required Monobehaviors")]
		[field: Required]
		public ProceduralMapSolver ProceduralMapSolver { get; set; }

		[field: SerializeField]
		[field: Required]
		public ProceduralTileSolver ProceduralTileSolver { get; set; }
	}

	public class ProceduralScaler : Singleton<ProceduralScaler, ProceduralScaler>,
	                                IEventListener<ProgressState>, IValidate {
		[SerializeField] [HideLabel] ProceduralScalerMonobehaviorModel _monoModel;

		bool ShouldFix => _monoModel != null && _monoModel.GetPropertyValues<object>().Any(x => x == null);

		void OnEnable() {
			this.StartListeningToEvents();
		}

		void OnDisable() {
			this.StopListeningToEvents();
		}

		public void OnEventHeard(ProgressState e) {
			if (e != ProgressState.ScalingGrid)
				return;

			SetGridOrigin();
			SetGridScale();
		}

		void IValidate.ValidateShouldQuit() {
			var exitHandler = new ProceduralExitHandler();

			var statements = new HashSet<Func<bool>> {
				() => _monoModel                      == null,
				() => _monoModel.ProceduralMapSolver  == null,
				() => _monoModel.ProceduralTileSolver == null
			};

			exitHandler.DetermineQuit(statements.ToArray());
		}

		void SetGridOrigin() {
			var mapConfig        = _monoModel.ProceduralMapSolver.MonoModel.ProceduralProceduralMapConfig;
			var tileSceneObjects = _monoModel.ProceduralTileSolver.MonoModel.TileMapGameObjects;

			tileSceneObjects.GridObject.gameObject.transform.position = ProcessNewPosition(mapConfig.MapSizeInt);
		}

		static Vector3 ProcessNewPosition(int2 mapSize) => new(
			Mathf.CeilToInt(-mapSize.x  / 2f),
			Mathf.FloorToInt(-mapSize.y / 2f),
			0);

		void SetGridScale() {
			var mapConfig        = _monoModel.ProceduralMapSolver.MonoModel.ProceduralProceduralMapConfig;
			var tileSceneObjects = _monoModel.ProceduralTileSolver.MonoModel.TileMapGameObjects;

			tileSceneObjects.GridObject.gameObject.transform.localScale = ProcessNewScale(mapConfig.CellSize);
		}

		static Vector3 ProcessNewScale(int cellSize) => new(cellSize, cellSize, cellSize);

		[Button]
		[ShowIf("@ShouldFix")]
		void Fix() {
			_monoModel.ProceduralMapSolver  = gameObject.FixComponent<ProceduralMapSolver>();
			_monoModel.ProceduralTileSolver = gameObject.FixComponent<ProceduralTileSolver>();
		}
	}
}