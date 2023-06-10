using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityBCL;
using UnityEngine;

namespace Procedural {
	public class BoxColliderSolver : ColliderSolver {
		const    string                         ZeroAngle          = "zeroAngle";
		const    string                         FortyFiveAngle     = "fortyFiveAngle";
		const    string                         OneThirtyFiveAngle = "oneThirtyFiveAngle";
		readonly Dictionary<string, GameObject> _colliderOwners;

		readonly BoxColliderModel _model;
		readonly GameObject       _root;

		public BoxColliderSolver(BoxColliderModel model, GameObject root) {
			_model          = model;
			_root           = root;
			_colliderOwners = new Dictionary<string, GameObject>();
		}

		public override async UniTask CreateCollider(ColliderSolverModel model) {
			CreateRotateColliderObject(ZeroAngle,          0f);
			CreateRotateColliderObject(FortyFiveAngle,     45f);
			CreateRotateColliderObject(OneThirtyFiveAngle, 135f);

			var roomObject = AddRoom(model.ColliderGameObject);
			roomObject.MakeStatic(true);

			for (var i = 0; i < model.Outlines.Count; i++) {
				var outline = model.Outlines[i];
				for (var j = 0; j < outline.Count; j++) {
					if (j + 1 >= outline.Count)
						continue;

					var point0 = model.WalkableVertices[outline[j]];
					var point1 = model.WalkableVertices[outline[j + 1]];

					var cx = (point0.x + point1.x) / 2f;
					var cy = (point0.y + point1.y) / 2f;
					var cz = (point0.z + point1.z) / 2f;

					var center = new Vector3(cx, cy, cz);

					var distance = Vector3.Distance(point0, point1);
					var col      = roomObject.AddComponent<BoxCollider>();

					col.center = center;
					col.size   = new Vector3(distance, _model.SkinWidth, 0f);
				}
			}

			roomObject.SetLayer(model.ObstacleLayer);
			await UniTask.Yield();
		}

		void CreateRotateColliderObject(string id, float angle) {
			var obj = new GameObject {
				name     = id,
				isStatic = true,
				transform = {
					eulerAngles = new Vector3(0, 0, angle),
					parent      = _root.transform
				}
			};

			_colliderOwners.Add(id, obj);
		}
	}
}