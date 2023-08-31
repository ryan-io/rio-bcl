using UnityEngine;

namespace Curves {
	public readonly struct CurveData {
		public static CurveData Build(Vector2 start, Vector2 tangentOne, Vector2 tangentTwo, Vector2 end)
			=> new CurveData(start, tangentOne, tangentTwo, end);

		public Vector2 Start      { get; }
		public Vector2 End        { get; }
		public Vector2 TangentOne { get; }
		public Vector2 TangentTwo { get; }

		public CurveData(Vector2 start, Vector2 tangentOne, Vector2 tangentTwo, Vector2 end) {
			Start      = start;
			End        = end;
			TangentOne = tangentOne;
			TangentTwo = tangentTwo;
		}
	}
}