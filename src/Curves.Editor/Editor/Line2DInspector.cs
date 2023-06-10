using UnityEditor;
using UnityEngine;

namespace Curves.Editor {
	[CustomEditor(typeof(Line2D))]
	public class Line2DInspector : PolynomialEditor {
		Line2D? _line;
		int     _selectedIndex = -1;

		Vector2 PointEndWorld
			=> Utility.ConvertPointToWorldSpace(HandleTransform, _line!.End);

		Vector2 PointStartWorld
			=> Utility.ConvertPointToWorldSpace(HandleTransform, _line!.Start);

		protected override void OnSceneGUI() {
			_line = (target as Line2D)!;

			if (!_line)
				return;

			ValidateDrawOptionals(_line);

			HandleTransform = _line.transform;
			HandleRotation  = GetRotation(HandleTransform);

			var pointStartWorld = PointStartWorld;
			var pointEndWorld   = PointEndWorld;

			var scalarStart = Utility.Handle.GetSize(pointStartWorld);
			var scalarEnd   = Utility.Handle.GetSize(pointEndWorld);

			CheckIfSelected(pointStartWorld, scalarStart, 0);
			CheckIfSelected(pointEndWorld,   scalarEnd,   1);

			SetColor(_line);
			DrawLine(pointStartWorld, pointEndWorld);

			pointStartWorld = CheckStart(pointStartWorld);
			pointEndWorld   = CheckEnd(pointEndWorld);

			if (_line.ShouldDrawLabels)
				LabelPoints(HandleTransform, _line.PointsReadonly, 0.3f, "");
		}

		Vector2 CheckEnd(Vector2 pointEndWorld) {
			if (_selectedIndex == 1) {
				EditorGUI.BeginChangeCheck();
				pointEndWorld = CreatePositionHandles(pointEndWorld, HandleRotation);

				if (EditorGUI.EndChangeCheck()) {
					Utility.Operations.Record(_line!);
					_line!.SetEnd(Utility.ConverToLocalSpace(HandleTransform, pointEndWorld));
				}
			}

			return pointEndWorld;
		}

		Vector2 CheckStart(Vector2 pointStartWorld) {
			if (_selectedIndex == 0) {
				EditorGUI.BeginChangeCheck();

				pointStartWorld = CreatePositionHandles(pointStartWorld, HandleRotation);

				if (EditorGUI.EndChangeCheck()) {
					Utility.Operations.Record(_line!);
					_line!.SetStart(Utility.ConverToLocalSpace(HandleTransform, pointStartWorld));
				}
			}

			return pointStartWorld;
		}

		void CheckIfSelected(Vector2 point, float scalar, int index) {
			if (Utility.Handle.IsSelected(point, HandleRotation, scalar)) {
				_selectedIndex = index;
				Repaint();
			}
		}
	}
}