using BCL;
using Source;
using Unity.Mathematics;
using UnityBCL;

namespace Procedural {
	public class ProceduralSceneBounds : Singleton<ProceduralSceneBounds, ISceneBounds>,
	                                     IEventListener<MapDimensionsModel> {
		Observable<int4>         _observable;
		public IObservable<int4> OnBoundaryDetermined => _observable;

		void Awake() {
			_observable = new Observable<int4>();
		}

		void OnEnable() {
			this.StartListeningToEvents();
		}

		void OnDisable() {
			this.StopListeningToEvents();
		}

		void IEventListener<MapDimensionsModel>.OnEventHeard(MapDimensionsModel e) {
			OnCompleteInvokeBoundaryEvent(e);
		}

		void OnCompleteInvokeBoundaryEvent(MapDimensionsModel model) {
			var dimensions = new MapDimensionsModel(
				model.MapWidth,
				model.MapHeight,
				model.BorderSize,
				model.CellSize);

			var bounds     = SceneHelper.GetTotalMapDimensions(dimensions);
			var xPos       = bounds[0] / 2;
			var xNeg       = -xPos;
			var yPos       = bounds[1] / 2;
			var yNeg       = -yPos;
			var boundaries = new int4(xPos, xNeg, yPos, yNeg);

			_observable.Signal(boundaries);
			_observable.Clear();
		}
	}
}