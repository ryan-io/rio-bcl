// BCL

using System;
using System.Globalization;
using UnityEngine;

namespace UnityBCL {
	[Serializable]
	public struct SerializableVector2 {
		public static implicit operator Vector2(SerializableVector2 v) => new(v.X, v.Y);
		public static explicit operator SerializableVector2(Vector2 v) => new(v.x, v.y);
		public                          float X                        { get; set; }
		public                          float Y                        { get; set; }

		public SerializableVector2(float x, float y) {
			X = x;
			Y = y;
		}

		public SerializableVector2(Vector2 vector) {
			X = vector.x;
			Y = vector.y;
		}
		
		public override string ToString() {
			return
				$"{X.ToString(CultureInfo.InvariantCulture)} " +
				$"{Y.ToString(CultureInfo.InvariantCulture)}  ";
		}

		public override int GetHashCode() {
			return HashCode.Combine(X, Y);
		}

		public override bool Equals(object obj) {
			return Equals(this);
		}

		public bool Equals(SerializableVector3 obj) {
			return Math.Abs(obj.X - X) < TOLERANCE &&
			       Math.Abs(obj.Y - Y) < TOLERANCE;
		}

		const float TOLERANCE = 0.0005f;
	}
}