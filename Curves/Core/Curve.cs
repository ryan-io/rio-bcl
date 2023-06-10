using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Curves {
	[HideMonoScript]
	[ExecuteInEditMode]
	public class Curve2D : Polynomial {
		protected override Vector2[] DefaultPoints =>
			Utility.Defaults.GetDefaultPoints(_polynomial == Power.Cubic
				                                  ? SplineType.CubicCurve
				                                  : SplineType.QuadraticCurve);

		public override SplineType SplineType
			=> _polynomial == Power.Cubic ? SplineType.CubicCurve : SplineType.QuadraticCurve;

		void OnValidate() {
			switch (_polynomial) {
				case Power.Quadratic:
					if (_points.Length != 3)
						Array.Resize(ref _points, 3);
					break;
				case Power.Cubic:
					if (_points.Length != 4)
						Array.Resize(ref _points, 4);
					break;
			}
		}

		public Vector2 GetPointQuadratic(float step) {
			var point = Utility.CurveUtil.GetPointQuadratic(_points[0], _points[1], _points[2], step);

			return transform.TransformPoint(point);
		}

		public Vector2 GetPointCubic(float step) {
			var point = Utility.CurveUtil.GetPointCubic(_points[0], _points[1], _points[2], _points[3], step);

			return transform.TransformPoint(point);
		}

		public Vector2 GetVelocityAtPointQuadratic(float step) {
			var point = Utility.CurveUtil.GetFirstDerivativeQuadratic(_points[0], _points[1], _points[2], step);
			var tr    = transform;

			return tr.TransformPoint(point) - tr.position;
		}

		public Vector2 GetVelocityAtPointCubic(float step) {
			var point = Utility.CurveUtil.GetFirstDerivativeCubic(_points[0], _points[1], _points[2], _points[3], step);
			var tr    = transform;

			return tr.TransformPoint(point) - tr.position;
		}

		void OnPowerChange() {
			_points = _polynomial == Power.Cubic
				          ? new[] { Vector2.zero, 2              * Vector2.one, 3 * Vector2.one, 4 * Vector2.one }
				          : new[] { Vector2.zero, Vector2.one, 2 * Vector2.one };
		}

#region Plumbing

		[VerticalGroup("Settings/Vertical", AnimateVisibility = true, PaddingBottom = 5f, PaddingTop = 5f)]
		[LabelText("Polynomial", true)]
		[PropertyOrder(-100)]
		[LabelWidth(200)]
		[Indent]
		[PropertySpace(10, 10)]
		[EnumToggleButtons]
		[OnValueChanged("OnPowerChange")]
		public Power _polynomial = Power.Quadratic;

		[TitleGroup("Point Extraction", "Control of lines, curves, & splines", Indent = true)]
		[HorizontalGroup("Point Extraction/SplineDisplays")]
		[LabelWidth(200)]
		[Indent]
		[PropertySpace(10, 0)]
		[GUIColor(84 / 255f, 195 / 255f, 255 / 255f)]
		[Button("Extract", ButtonSizes.Medium, ButtonStyle.CompactBox)]
		protected override void ExtractPoints() {
			var extractor = new CurvePointExtractor();
			extractor.EnableLogging();
			var data = extractor.Extract(this, _steps);

			_extractedPoints = data.Points;
		}

		[TitleGroup("Polynomial Shape", "Control of lines, curves, & splines", Indent = true)]
		[LabelWidth(200)]
		[Indent]
		[PropertySpace(10, 20)]
		[GUIColor(254 / 255f, 214 / 255f, 82 / 255f)]
		[Button("Reset", ButtonSizes.Medium, ButtonStyle.CompactBox)]
		protected override void Reset() {
			OnPowerChange();
#if UNITY_EDITOR || UNITY_STANDALONE

			Utility.Operations.Record(this, "Reset Curve 2D");

#endif
		}

#endregion
	}
}