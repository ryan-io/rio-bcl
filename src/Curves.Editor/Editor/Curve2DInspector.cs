using UnityEditor;
using UnityEngine;

namespace Curves.Editor {
	[CustomEditor(typeof(Curve2D))]
	public class Curve2DInspector : PolynomialEditor {
		Curve2D _curve         = null!;
		int     _selectedIndex = -1;

		protected override void OnSceneGUI() {
			_curve = (target as Curve2D)!;

			if (!_curve)
				return;

			ValidateDrawOptionals(_curve);

			HandleTransform = _curve.transform;
			HandleRotation  = GetRotation(HandleTransform);

			if (_curve.ShouldDrawLabels)
				LabelPoints(HandleTransform, _curve.PointsReadonly, 0.3f, "Tangent");

			if (_curve.DrawHandleLines) {
				var connectingPoint = DrawQuadraticLines();

				if (_curve._polynomial == Power.Cubic)
					DrawCubicLines(connectingPoint);
			}

			SetColor(_curve);
			DrawCurve();
		}

		void DrawCurve() {
			switch (_curve._polynomial) {
				case Power.Quadratic:
					Utility.CurveUtil.DrawCurveQuadratic(_curve);
					break;
				case Power.Cubic:
					Utility.CurveUtil.DrawCurveCubic(_curve);
					break;
			}
		}

		Vector2 DrawQuadraticLines() {
			var point0 = DrawPoint(0);
			var point1 = DrawPoint(1);
			var point2 = DrawPoint(2);

			DrawLine(point0, point1);
			DrawLine(point1, point2);

			return point2;
		}

		void DrawCubicLines(Vector2 connectingPoint) {
			var point3 = DrawPoint(3);
			DrawLine(connectingPoint, point3);
		}

		Vector2 DrawPoint(int index) {
			var point  = Utility.ConvertPointToWorldSpace(HandleTransform, _curve.GetPoint(index));
			var scalar = Utility.Handle.GetSize(point);

			if (Utility.Handle.IsSelected(point, HandleRotation, scalar)) {
				_selectedIndex = index;
				Repaint();
			}

			if (_selectedIndex == index) {
				EditorGUI.BeginChangeCheck();
				point = CreatePositionHandles(point, HandleRotation);

				if (EditorGUI.EndChangeCheck()) {
					Utility.Operations.Record(_curve, "Move Bezier Spline Point");
					point = Utility.ConverToLocalSpace(HandleTransform, point);
					_curve.SetPoint(index, point);
				}
			}

			return point;
		}
	}
}