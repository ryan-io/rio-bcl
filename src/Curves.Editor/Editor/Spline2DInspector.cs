#if UNITY_STANDALONE || UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Curves.Editor {
	[CustomEditor(typeof(Spline2D))]
	public class Spline2DInspector : PolynomialEditor {
		int _selectedIndex = -1;

		Spline2D _spline = null!;

		protected override void OnSceneGUI() {
			_spline = (target as Spline2D)!;

			if (!_spline)
				return;

			ValidateDrawOptionals(_spline);

			HandleTransform = _spline!.transform;
			HandleRotation  = GetRotation(HandleTransform);

			SetColor(_spline);

			if (_spline.ShouldDrawLabels)
				LabelPoints(HandleTransform, _spline.PointsReadOnly, 0.3f, "Tangent", _spline.Loop);

			ProcessPoints();
			DrawConnections();
		}

		bool HandleIsSelected(Vector2 point, float scalar)
			=> Utility.Handle.IsSelected(point, HandleRotation, scalar);

		void ProcessPoints() {
			var pointKnot = DrawPoint(0);

			for (var i = 1; i < _spline!.PointCount; i += 3) {
				var point1 = DrawPoint(i);
				var point2 = DrawPoint(i + 1);
				var point3 = DrawPoint(i + 2);


				if (_spline.DrawHandleLines) {
					DrawLine(pointKnot, point1);
					DrawLine(point2,    point3);
				}

				Handles.DrawBezier(
					pointKnot, point3,
					point1, point2,
					_spline.SerializedColor, null, 2f);

				pointKnot = point3;
			}
		}

		void DrawConnections() {
			EditorGUI.BeginChangeCheck();

			var connectionMode = _spline!.GetConnection(_selectedIndex);

			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(_spline, "Change Mode Type");
				_spline.SetConnection(_selectedIndex, connectionMode);
				EditorUtility.SetDirty(_spline);
			}
		}

		Vector2 DrawPoint(int index) {
			var point  = Utility.ConvertPointToWorldSpace(HandleTransform, _spline!.GetPoint(index));
			var scalar = Utility.Handle.GetSize(point);

			Handles.color = index == 0
				                ? Utility.Colors.SalmonPink
				                : Utility.Colors.GetConnectionColor(_spline.GetConnection(index + 1));

			if (HandleIsSelected(point, scalar)) {
				_selectedIndex = index;
				Repaint();
			}

			if (_selectedIndex == index) {
				EditorGUI.BeginChangeCheck();
				point = CreatePositionHandles(point, HandleRotation);

				if (EditorGUI.EndChangeCheck()) {
					Utility.Operations.Record(_spline, "Move Bezier Spline Point");
					point = Utility.ConverToLocalSpace(HandleTransform, point);
					_spline.SetPoint(index, point);
				}
			}

			return point;
		}
	}
}