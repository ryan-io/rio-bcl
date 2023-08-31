using UnityEngine;

// by Pepijn Willekens
// https://twitter.com/PepijnWillekens
// pepijnwillekens@gmail.com
namespace ProceduralAuxiliary.ProceduralCollider {
	public static class VectorExtension {
		public delegate float FloatEdit(float input);
		//Vector3

		public static Vector3 ChangeX(this Vector3 parent, float newX) => new(newX, parent.y, parent.z);

		public static Vector3 ChangeY(this Vector3 parent, float newY) => new(parent.x, newY, parent.z);

		public static Vector3 ChangeZ(this Vector3 parent, float newZ) => new(parent.x, parent.y, newZ);

		public static Vector3 ChangeX(this Vector3 parent, FloatEdit edit) => new(edit(parent.x), parent.y, parent.z);

		public static Vector3 ChangeY(this Vector3 parent, FloatEdit edit) => new(parent.x, edit(parent.y), parent.z);

		public static Vector3 ChangeZ(this Vector3 parent, FloatEdit edit) => new(parent.x, parent.y, edit(parent.z));
		//Vector2

		public static Vector2 ChangeX(this Vector2 parent, float newX) => new(newX, parent.y);

		public static Vector2 ChangeY(this Vector2 parent, float newY) => new(parent.x, newY);

		public static Vector2 ChangeX(this Vector2 parent, FloatEdit edit) => new(edit(parent.x), parent.y);

		public static Vector2 ChangeY(this Vector2 parent, FloatEdit edit) => new(parent.x, edit(parent.y));

		public static void SetX(this Vector3 v, float newX) {
			v.x = newX;
		}

		public static void SetY(this Vector3 v, float newY) {
			v.y = newY;
		}

		public static void SetZ(this Vector3 v, float newZ) {
			v.z = newZ;
		}

		public static void SetX(this Vector2 v, float newX) {
			v.x = newX;
		}

		public static void SetY(this Vector2 v, float newY) {
			v.y = newY;
		}

		public static string ToDetailedString(this Vector2 v) => v.ToString("F5");

		public static string ToDetailedString(this Vector3 v) => v.ToString("F5");

		public static Vector3 AddX(this Vector3 parent, float changeX) => new(parent.x + changeX, parent.y, parent.z);

		public static Vector3 AddY(this Vector3 parent, float changeY) => new(parent.x, parent.y + changeY, parent.z);

		public static Vector3 AddZ(this Vector3 parent, float changeZ) => new(parent.x, parent.y, parent.z + changeZ);

		public static Vector2 AddX(this Vector2 parent, float changeX) => new(parent.x + changeX, parent.y);

		public static Vector2 AddY(this Vector2 parent, float changeY) => new(parent.x, parent.y + changeY);

		public static Vector2 Abs(this Vector2 v) => new(Mathf.Abs(v.x), Mathf.Abs(v.y));

		public static Vector3 Abs(this Vector3 v) => new(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

		public static Vector3 ChangeXY(this Vector3 v, Vector2 xy) => new(xy.x, xy.y, v.z);

		public static Vector3 AddXY(this Vector3 v, Vector2 xy) => new(v.x + xy.x, v.y + xy.y, v.z);

		public static Vector3 ToVector3(this Vector2 v) => new(v.x, v.y, 0);

		public static Vector3 ToVector3(this Vector2 v, float z) => new(v.x, v.y, z);
	}
}