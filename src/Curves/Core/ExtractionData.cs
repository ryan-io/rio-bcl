using UnityEngine;

namespace Curves {
	public readonly struct ExtractionData {
		public static ExtractionData GetNewEmpty() => new ExtractionData();

		public Vector2[] Points { get; }

		public int StepSize { get; }

		public ExtractionData(Vector2[] points, int stepSize) {
			Points   = points;
			StepSize = stepSize;
		}
	}
}