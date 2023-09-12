using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace UnityBCL {
	public static class VectorExtensionMethods {
		public static Vector2 ToVector2(this System.Numerics.Vector2 v) => new(v.X, v.Y);
		public static Vector3 ToVector3(this System.Numerics.Vector3 v) => new(v.X, v.Y, v.Z);

		public static System.Numerics.Vector2 ToVector2(this Vector2 v) => new(v.x, v.y);
		public static System.Numerics.Vector3 ToVector3(this Vector3 v) => new(v.x, v.y, v.z);

		public static IEnumerable<System.Numerics.Vector2> AsIEnumerable(this IEnumerable<Vector2> v)
			=> v.Select(vector => new System.Numerics.Vector2(vector.x, vector.y));

		public static IEnumerable<System.Numerics.Vector3> AsIEnumerable(this IEnumerable<Vector3> v)
			=> v.Select(vector => new System.Numerics.Vector3(vector.x, vector.y, vector.z));

		public static IEnumerable<Vector2> AsIEnumerable(this IEnumerable<System.Numerics.Vector2> v)
			=> v.Select(vector => new Vector2(vector.X, vector.Y));

		public static IEnumerable<Vector3> AsIEnumerable(this IEnumerable<System.Numerics.Vector3> v)
			=> v.Select(vector => new Vector3(vector.X, vector.Y, vector.Z));

		public static List<Vector2> ToList(this NativeArray<System.Numerics.Vector2> nA) {
			var list = new List<Vector2>(nA.Length);
			list.AddRange(nA.Select(item => item.ToVector2()));

			return list;
		}

		public static List<Vector3> ToList(this NativeArray<System.Numerics.Vector3> nA) {
			var list = new List<Vector3>(nA.Length);
			list.AddRange(nA.Select(item => item.ToVector3()));

			return list;
		}

		public static Vector2 ToVector2(this Vector2Int v) => new(v.x, v.y);
		public static Vector3 ToVector3(this Vector2Int v) => new(v.x, v.y, 0f);
		public static Vector3 ToVector3(this Vector3Int v) => new(v.x, v.y, 0f);

		public static IEnumerable<SerializableVector3> AsSerialized(this Vector3[] l) {
			if (l.IsEmptyOrNull())
				return Enumerable.Empty<SerializableVector3>();

			return l.Select(v => new SerializableVector3(v.x, v.y, v.z));
		}

		public static IEnumerable<SerializableVector3> AsSerialized(this List<Vector3> l) {
			if (l.IsEmptyOrNull())
				return Enumerable.Empty<SerializableVector3>();

			return l.Select(v => new SerializableVector3(v.x, v.y, v.z));
		}

		public static IEnumerable<SerializableVector2> AsSerialized(this Vector2[] l) {
			if (l.IsEmptyOrNull())
				return Enumerable.Empty<SerializableVector2>();

			return l.Select(v => new SerializableVector2(v.x, v.y));
		}


		public static IEnumerable<SerializableVector2> AsSerialized(this List<Vector2> l) {
			if (l.IsEmptyOrNull())
				return Enumerable.Empty<SerializableVector2>();

			return l.Select(v => new SerializableVector2(v.x, v.y));
		}

		public static IEnumerable<Vector3> Deserialized(this SerializableVector3[] l) {
			if (l.IsEmptyOrNull())
				return Enumerable.Empty<Vector3>();

			return l.Select(v => new Vector3(v.X, v.Y, v.Z));
		}

		public static IEnumerable<Vector3> Deserialized(this List<SerializableVector3> l) {
			if (l.IsEmptyOrNull())
				return Enumerable.Empty<Vector3>();

			return l.Select(v => new Vector3(v.X, v.Y, v.Z));
		}

		public static IEnumerable<Vector2> Deserialized(this SerializableVector2[] l) {
			if (l.IsEmptyOrNull())
				return Enumerable.Empty<Vector2>();

			return l.Select(v => new Vector2(v.X, v.Y));
		}

		public static IEnumerable<Vector2> Deserialized(this List<SerializableVector2> l) {
			if (l.IsEmptyOrNull())
				return Enumerable.Empty<Vector2>();

			return l.Select(v => new Vector2(v.X, v.Y));
		}
	}
}