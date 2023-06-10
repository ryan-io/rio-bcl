#if UNITY_EDITOR || UNITY_STANDALONE
using UnityEditor;
using UnityEngine;

namespace Procedural.Editor {
	[CustomEditor(typeof(ProceduralPrimitiveCollider))]
	internal class PolygonColliderEditor : UnityEditor.Editor {
		protected virtual void OnSceneGUI() {
			var proceduralPrimitiveCollider = (ProceduralPrimitiveCollider)target;
			if (!proceduralPrimitiveCollider.CanBeEdited() || !proceduralPrimitiveCollider.isActiveAndEnabled) return;
			var corners = proceduralPrimitiveCollider.corners;
			if (corners.Count < 3) return;
			for (var i = 0; i < corners.Count + proceduralPrimitiveCollider.loopInt(); i++) {
				var from = corners[i].position;
				var to   = corners[(i + 1) % corners.Count].position;
				DrawButton(proceduralPrimitiveCollider, from, to, corners[i], corners[i].GetSiblingIndex() + 1);
			}

			if (!proceduralPrimitiveCollider.loop) {
				DrawButton(proceduralPrimitiveCollider,                                corners[0].position,
					corners[0].position + (corners[0].position - corners[1].position), corners[0],
					corners[0].GetSiblingIndex());
				var lastIx = corners.Count - 1;
				DrawButton(proceduralPrimitiveCollider, corners[lastIx].position,
					corners[lastIx].position + (corners[lastIx].position - corners[lastIx - 1].position),
					corners[lastIx], corners[lastIx].GetSiblingIndex() + 1);
			}
		}

		public override void OnInspectorGUI() {
			var proceduralPrimitiveCollider = (ProceduralPrimitiveCollider)target;
			if (proceduralPrimitiveCollider.CanBeEdited()) {
				base.OnInspectorGUI();
				if (proceduralPrimitiveCollider.DEBUG)
					EditorGUILayout.HelpBox(
						"Use debug mode at your own risk. We cannot guarantee everything to stay error-free in debug mode, as this allows you to break things. It's only intended for investigation and debugging, if you know what you're doing.",
						MessageType.Warning);
			}
			else {
				GUILayout.Label(
					"Editing polygon colliders is not allowed on Prefabs. Open the prefab and edit there instead");
			}
		}

		void DrawButton(ProceduralPrimitiveCollider proceduralPrimitiveCollider, Vector3 from, Vector3 to,
			Transform cornerPrototype,
			int newIndex) {
			var prevColor = Handles.color;
			Handles.color = ((ProceduralPrimitiveCollider)target).GizmoColor;

			var size = Vector3.Distance(from, to) / 20f;
			if (Handles.Button(((from            + to) / 2).ChangeY(y => y - 0.001f),
				    Quaternion.LookRotation(from - to) * Quaternion.Euler(90, 0, 0), size, size,
				    Handles.ConeHandleCap)) {
				var newCorner = Instantiate(cornerPrototype, (from + to) / 2, Quaternion.identity,
					proceduralPrimitiveCollider.transform);
				newCorner.SetSiblingIndex(newIndex);
				Selection.activeGameObject = newCorner.gameObject;
			}

			Handles.color = prevColor;
		}
	}
}
#endif