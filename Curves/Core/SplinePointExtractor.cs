using UnityEngine;

namespace Curves {
	public class SplinePointExtractor : PointExtractor<Spline2D> {
		IndexBuilder _indexBuilder;
		int          _iterativeOffsetIndex;

		Vector2 LastIteratedPoint {
			get => ExtractedPoints[ExtractedPoints.Length - 1];
			set => ExtractedPoints[ExtractedPoints.Length - 1] = value;
		}

		int CurveIterator => _iterativeOffsetIndex + _divisionsPerCurve;

		public override ExtractionData Extract(Spline2D polynomial, int numOfDivisions) {
			if (GuardAgainstExtraction(numOfDivisions))
				return ExtractionData.GetNewEmpty();

			var indexBuilder = new IndexBuilder(4, 3);

			SetDivisionsPerCurve(polynomial, numOfDivisions);
			SetStep();
			ResizeArray(numOfDivisions);
			ResetIterativeIndex();

			for (var x = 1; x <= polynomial.CurveCount; x++)
				BuildPointArray(polynomial, indexBuilder);

			VerifyEnd(polynomial);

			return new ExtractionData(ExtractedPoints, numOfDivisions);
			;
		}

		protected override void SetDivisionsPerCurve(Spline2D spline, int numberOfDivisions)
			=> _divisionsPerCurve = numberOfDivisions / spline.CurveCount;

		void BuildPointArray(Spline2D polynomial, IndexBuilder indexBuilder) {
			var indexArray = indexBuilder.GetIndexArray();

			ResetProgress();

			var curve = BuildCurve(
				polynomial.GetPoint(indexArray[0]), polynomial.GetPoint(indexArray[1]),
				polynomial.GetPoint(indexArray[2]), polynomial.GetPoint(indexArray[3]));

			for (var i = _iterativeOffsetIndex; i <= CurveIterator; i++, _progress = Mathf.Clamp01(_progress + _step)) {
				_progress = VerifyProgress(_progress);
				GetAndSetPoint(polynomial, curve, _progress, i);
			}

			SetIterativeIndex();
			indexBuilder.Increment();
		}

		void GetAndSetPoint(Spline2D spline, CurveData curveData, float step, int index) {
			var point = spline.GetPointOnSpline(curveData, step);
			SetPoint(point, index);
		}

		void SetIterativeIndex()
			=> _iterativeOffsetIndex += _divisionsPerCurve + 1;

		void ResetIterativeIndex()
			=> _iterativeOffsetIndex = 0;

		void VerifyEnd(Spline2D spline) {
			var isCloseEnough = Utility.LineUtil.IsWithinError(spline.LastPoint, LastIteratedPoint, 0.05f);

			if (!isCloseEnough)
				LastIteratedPoint = spline.LastPoint;
		}

		CurveData BuildCurve(Vector2 start, Vector2 t1, Vector2 t2, Vector2 end)
			=> CurveData.Build(start, t1, t2, end);
	}
}