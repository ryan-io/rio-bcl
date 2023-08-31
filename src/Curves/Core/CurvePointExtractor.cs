using UnityEngine;

namespace Curves {
	public class CurvePointExtractor : PointExtractor<Curve2D> {
		public override ExtractionData Extract(Curve2D polynomial, int numOfDivisions) {
			if (GuardAgainstExtraction(numOfDivisions))
				return ExtractionData.GetNewEmpty();

			SetDivisionsPerCurve(polynomial, numOfDivisions);
			SetStep();
			ResizeArray(numOfDivisions + 1);
			ResetProgress();

			if (polynomial._polynomial == Power.Cubic)
				SetPointsCubic(polynomial);

			else
				SetPointsQuadratic(polynomial);

			return new ExtractionData(ExtractedPoints, _divisionsPerCurve);
		}

		void SetPointsCubic(Curve2D curve) {
			for (var i = 0; i <= _divisionsPerCurve; i++, _progress = Mathf.Clamp01(_progress + _step)) {
				_progress = VerifyProgress(_progress);
				var point = curve.GetPointCubic(_progress);
				SetPoint(point, i);
			}
		}

		void SetPointsQuadratic(Curve2D curve) {
			for (var i = 0; i <= _divisionsPerCurve; i++, _progress = Mathf.Clamp01(_progress + _step)) {
				var point = curve.GetPointQuadratic(_progress);
				SetPoint(point, i);
			}
		}

		protected override void SetDivisionsPerCurve(Curve2D spline, int numberOfDivisions) {
			if (spline._polynomial == Power.Cubic)
				_divisionsPerCurve = numberOfDivisions;

			else if (spline._polynomial == Power.Quadratic)
				_divisionsPerCurve = spline._steps;
		}
	}
}