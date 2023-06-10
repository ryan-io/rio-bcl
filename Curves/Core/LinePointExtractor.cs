using UnityEngine;

namespace Curves {
	public class LinePointExtractor : PointExtractor<Line2D> {
		public override ExtractionData Extract(Line2D polynomial, int numOfDivisions) {
			if (GuardAgainstExtraction(numOfDivisions))
				return ExtractionData.GetNewEmpty();

			SetDivisionsPerCurve(polynomial, numOfDivisions);
			SetStep();
			ResizeArray(numOfDivisions + 1);
			ResetProgress();

			SetPoints(polynomial);

			return new ExtractionData(ExtractedPoints, _divisionsPerCurve);
		}

		protected override void SetDivisionsPerCurve(Line2D spline, int numberOfDivisions)
			=> _divisionsPerCurve = numberOfDivisions;

		void SetPoints(Line2D line) {
			for (var i = 0; i <= _divisionsPerCurve; i++, _progress = Mathf.Clamp01(_progress + _step)) {
				_progress = VerifyProgress(_progress);
				var point = line.GetPointOnLine(_progress);
				SetPoint(point, i);
			}
		}
	}
}