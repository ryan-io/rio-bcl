using Cysharp.Threading.Tasks;
using UnityBCL;
using UnityEngine;

namespace Procedural {
	public class PrimitiveColliderSolver : ColliderSolver {
		readonly PrimitiveColliderModel _model;

		public PrimitiveColliderSolver(PrimitiveColliderModel model) => _model = model;

		public override UniTask CreateCollider(ColliderSolverModel model) {
			model.ColliderGameObject.MakeStatic(true);
			model.ColliderGameObject.ZeroPosition();

			for (var i = 0; i < model.Outlines.Count; i++) {
				var col              = CreateNewPrimitiveCollider(model.ColliderGameObject, i.ToString());
				var outline          = model.Outlines[i];
				var extractedCorners = col.corners;

				for (var k = 0; k < 3; k++) {
					var newPoint = new Vector3(
						model.WalkableVertices[outline[k]].x, model.WalkableVertices[outline[k]].y, 0);
					extractedCorners[k].transform.position = newPoint;
					extractedCorners[k].gameObject.MakeStatic(true);
				}

				for (var j = 3; j < outline.Count; j++) {
					var newPoint = new Vector3(
						model.WalkableVertices[outline[j]].x, model.WalkableVertices[outline[j]].y, 0);
					CreateHandle(col, newPoint, col.corners[^1], col.corners[^1].GetSiblingIndex() + 1);
				}
			}

			model.ColliderGameObject.SetLayerRecursive(LayerMask.NameToLayer("Boundary"));

			return new UniTask();
		}

		ProceduralPrimitiveCollider CreateNewPrimitiveCollider(GameObject parent, string identifier) {
			var obj = new GameObject {
				name = $"Primitive Collider - Room {identifier}",
				transform = {
					position = Vector3.zero,
					parent   = parent.transform
				}
			};

			obj.MakeStatic(true);
			obj.transform.eulerAngles = new Vector3(90, 0, 0);

			var col = obj.AddComponent<ProceduralPrimitiveCollider>();
			InjectSettings(col);

			for (var i = 0; i < 3; i++) {
				var newObj = new GameObject().transform;
				newObj.SetParent(col.gameObject.transform);
				newObj.localPosition   = Vector3.forward * (0.5f * 5 * i);
				newObj.gameObject.name = i.ToString();
#if UNITY_EDITOR|| UNITY_STANDALONE
				//PolygonColliderEditorExtention.DrawIcon(newObj.gameObject, 0);
#endif
				col.corners.Add(newObj);
			}

			//orcePopulateCorners();
			return col;
		}

		void InjectSettings(ProceduralPrimitiveCollider col) {
			col.depth            = _model.SkinWidth / 2f;
			col.heigth           = _model.SkinWidth / 2f;
			col.onlyWhenSelected = true;
			col.radius           = _model.Radius;
		}

		void CreateHandle(
			Component easyWallCollider, Vector3 newPos, Transform cornerPrototype, int newIndex) {
			var newCorner = Object.Instantiate(
				cornerPrototype, newPos, Quaternion.identity, easyWallCollider.transform);

			newCorner.gameObject.MakeStatic(true);
			newCorner.SetSiblingIndex(newIndex);
		}
	}
}