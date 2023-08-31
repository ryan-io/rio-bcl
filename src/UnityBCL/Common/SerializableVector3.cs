using System;
using System.Globalization;
using UnityEngine;

namespace UnityBCL {
	[Serializable]
	public struct SerializableVector3 {
		public static implicit operator Vector3(SerializableVector3 v) => new(v.X, v.Y, v.Z);
		public static explicit operator SerializableVector3(Vector3 v) => new(v.x, v.y, v.z);

		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }

		public SerializableVector3(float x, float y, float z) {
			X = x;
			Y = y;
			Z = z;
		}

		public SerializableVector3(Vector3 vector) {
			X = vector.x;
			Y = vector.y;
			Z = vector.z;
		}

		public override string ToString() {
			return
				$"{X.ToString(CultureInfo.InvariantCulture)} "   +
				$"${Y.ToString(CultureInfo.InvariantCulture)}  " +
				$"${Z.ToString(CultureInfo.InvariantCulture)}";
		}

		public override int GetHashCode() {
			return HashCode.Combine(X, Y, Z);
		}

		public override bool Equals(object obj) {
			return Equals(this);
		}

		public bool Equals(SerializableVector3 obj) {
			return Math.Abs(obj.X - X) < TOLERANCE &&
			       Math.Abs(obj.Y - Y) < TOLERANCE &&
			       Math.Abs(obj.Z - Z) < TOLERANCE;
		}

		const float TOLERANCE = 0.0005f;
	}
}