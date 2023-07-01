using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Curves {
	[HideMonoScript]
	public class Line2D : Polynomial {
		public Vector2 Start
			=> _points[0];

		public Vector2 End
			=> _points[1];

		public override SplineType SplineType
			=> SplineType.Line;

		protected override Vector2[] DefaultPoints
			=> Utility.Defaults.GetDefaultPoints(SplineType.Line);

		void OnValidate()
			=> ConstrainPointsArray();

		public void SetStart(Vector2 vector)
			=> _points[0] = vector;

		public void SetEnd(Vector2 vector)
			=> _points[1] = vector;

		public Vector2 GetPointOnLine(float step) {
			if (step < 0 || step > 1) {
				Utility.Log.Warning(OutOfRange);
				return default;
			}

			return Utility.LineUtil.GetInterpolatedPoint(transform, Start, End, step);
		}

		void ConstrainPointsArray() {
			if (_points.Length != 2)
				Array.Resize(ref _points, 2);
		}

#region Plumbing

		[TitleGroup("Point Extraction", "Control of lines, curves, & splines", Indent = true)]
		[HorizontalGroup("Point Extraction/SplineDisplays")]
		[LabelWidth(200)]
		[Indent]
		[PropertySpace(10, 0)]
		[GUIColor(84 / 255f, 195 / 255f, 255 / 255f)]
		[Button("Extract", ButtonSizes.Medium, ButtonStyle.CompactBox)]
		protected override void ExtractPoints() {
			var extractor = new LinePointExtractor();
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
			_points = new[] { Vector2.zero, 2 * Vector2.one };

			//Utility.Operations.Record(this, "Reset Line 2D");  -> record to undo stack
		}

		const string OutOfRange = "Please provide a value between 0 and 1. 0 and 1 are inclusive.";

#endregion
	}
}