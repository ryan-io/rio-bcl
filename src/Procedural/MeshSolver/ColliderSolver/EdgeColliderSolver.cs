using Cysharp.Threading.Tasks;
using UnityBCL;
using UnityEngine;

namespace Procedural {
	public class EdgeColliderSolver : ColliderSolver {
		readonly EdgeCollider2D[] _edgeColliders;

		readonly EdgeColliderModel _model;

		public EdgeColliderSolver(EdgeColliderModel model, EdgeCollider2D[] edgeColliders) {
			Logger         = new UnityLogging(new object());
			_model         = model;
			_edgeColliders = edgeColliders;
		}

		UnityLogging Logger { get; }

		public override UniTask CreateCollider(ColliderSolverModel model) {
			model.ColliderGameObject.ZeroPosition();
			model.ColliderGameObject.MakeStatic(false);

			Logger.Test($"Veriying the number of rooms: {model.Outlines.Count}");
			for (var i = 0; i < model.Outlines.Count; i++) {
				var roomObject = AddRoom(model.ColliderGameObject);
				roomObject.MakeStatic(true);

				if (IsValidLayer(model.ObstacleLayer))
					roomObject.SetLayer(model.ObstacleLayer);
				else
					Logger.Warning("Could not set " + roomObject.name + " to layer " + (int)model.ObstacleLayer);

				var outline      = model.Outlines[i];
				var edgeCollider = roomObject.AddComponent<EdgeCollider2D>();
				var edgePoints   = new Vector2[outline.Count];
				var count        = outline.Count;
				edgeCollider.offset = _model.EdgeColliderOffset;

				for (var j = 0; j < count; j++)
					edgePoints[j] = new Vector2(model.WalkableVertices[outline[j]].x,
						model.WalkableVertices[outline[j]].y);

				edgeCollider.points = edgePoints;

				if (i >= _edgeColliders.Length)
					continue;

				_edgeColliders[i] = edgeCollider;
			}

			SetColliderRadius(model);

			return new UniTask();
		}

		void SetColliderRadius(ColliderSolverModel model) {
			var count = model.Outlines.Count;

			for (var i = 0; i < count; i++) {
				if (i >= _edgeColliders.Length)
					continue;
				_edgeColliders[i].edgeRadius = _model.Radius;
			}
		}

		bool IsValidLayer(int layerMask) => layerMask is >= 0 and <= 31;
	}
}