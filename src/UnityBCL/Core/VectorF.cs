using UnityEngine;
using r = UnityEngine.Random;

namespace UnityBCL {
	public static class VectorF {
		const float Tolerance = 0.005f;

		public static Vector3 AddRandom(this Vector3 v, Limits limits) => v + GetRandomVector(limits);

		public static Vector3 GetRandomVector(Limits limits) {
			var randomX = r.Range(limits.XMin, limits.XMax);
			var randomY = r.Range(limits.YMin, limits.YMax);
			var randomZ = r.Range(limits.ZMin, limits.ZMax);

			return new Vector3(randomX, randomY, randomZ);
		}

		public static Quaternion NewQuatFromV3(Vector3 vector, float representationScalar = 1f)
			=> new(vector.x, vector.y, vector.z, representationScalar);

		public static Vector2[] Vector3ArrayToVector2Array(Vector3[] array) {
			var count        = array.Length;
			var vector2Array = new Vector2[count];

			for (var i = 0; i < count; i++)
				vector2Array[i] = array[i];

			return vector2Array;
		}

		public static Vector3[] Vector2ArrayToVector3Array(Vector2[] array) {
			var count        = array.Length;
			var vector3Array = new Vector3[count];

			for (var i = 0; i < count; i++)
				vector3Array[i] = array[i];

			return vector3Array;
		}

		public static Vector3 DirectionTo(Vector3 to, Vector3 from) => (to - from).normalized;

		public static Vector3 ScaleVector(Vector3 vectorToTarget, float scalar) => scalar * vectorToTarget;

		public static float DistanceTo(Vector3 from, Vector3 to) => Vector3.Distance(from, to);

		public static bool DistanceToIsLessThanOrEqualTo(Vector3 from, Vector3 to, float comparison) =>
			Vector2.Distance(from, to) <= comparison;

		public static bool DistanceToIsGreaterThanOrEqualTo(Vector3 from, Vector3 to, float comparison) =>
			Vector2.Distance(from, to) >= comparison;

		public static bool AbsoluteDifferenceLessThan(float from, float to, float comparisonValue) =>
			Mathf.Abs(from - to) < comparisonValue;

		public static bool AbsoluteDifferenceGreaterThan(float from, float to, float comparisonValue) =>
			Mathf.Abs(from - to) > comparisonValue;

		public static bool AbsoluteDifferenceEqualsToWithError(float from, float to, float comparisonValue, float error)
			=> Mathf.Abs(from - to) <= error;

		public static float DotProduct(Vector3 fromHeading, Vector3 toHeading)
			=> Vector3.Dot(fromHeading.normalized, toHeading.normalized);

		public static bool IsInFov(Vector3 from, Vector3 to, float fovAngle) {
			var dot = DotProduct(from, to);
			return dot >= Mathf.Cos(fovAngle);
		}

		public static Vector3 WorldPosition(Vector3 localPosition) => default;

		public readonly struct Limits {
			public float XMin { get; }
			public float YMin { get; }
			public float ZMin { get; }
			public float XMax { get; }
			public float YMax { get; }
			public float ZMax { get; }

			public Limits(float xMin, float xMax, float yMin, float yMax, float zMin, float zMax) {
				XMin = xMin;
				XMax = xMax;
				YMin = yMin;
				YMax = yMax;
				ZMin = zMin;
				ZMax = zMax;
			}
		}
	}
}