using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BCL {
	public static class CollectionExtensionMethods {
		public static bool IsNullOrEmpty<TKey, TValue>(this Dictionary<TKey, TValue> dict) where TKey : notnull {
			return !dict.GetEnumerator().MoveNext();
		}

		public static bool IsNullOrEmpty<T>(this T[] collection)
			=> !collection.GetEnumerator().MoveNext();

		public static bool IsNullOrEmpty<T>(this List<T> list)
			=> !list.GetEnumerator().MoveNext();

		public static bool IsNullOrEmpty(this IEnumerable enumerable)
			=> !enumerable.GetEnumerator().MoveNext();

		public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
			=> !enumerable.Any();

		public static bool IsEmpty<T>(this HashSet<T> hashSet) => hashSet.Count < 1;


		public static List<T> GetAllPublicConstantValues<T>(this Type type) {
			return type
			      .GetFields(ConstantFlags)
			      .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
			      .Select(x => (T)x.GetRawConstantValue()!)
			      .ToList();
		}


		const BindingFlags ConstantFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
	}
}