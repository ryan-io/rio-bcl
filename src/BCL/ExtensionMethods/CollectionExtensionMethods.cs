using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RIO.BCL {
	public static class CollectionExtensionMethods {
		const BindingFlags ConstantFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;

		public static bool NotAcceptable<TKey, TValue>(this Dictionary<TKey, TValue> dict) where TKey : notnull
			=> !dict.GetEnumerator().MoveNext();

		public static bool NotAcceptable<T>(this T[] collection)
			=> !collection.GetEnumerator().MoveNext();

		public static bool NotAcceptable<T>(this List<T> list)
			=> !list.GetEnumerator().MoveNext();

		public static bool NotAcceptable(this IEnumerable enumerable)
			=> !enumerable.GetEnumerator().MoveNext();

		public static bool NotAcceptable<T>(this IEnumerable<T> enumerable)
			=> !enumerable.Any();

		public static bool IsEmpty<T>(this HashSet<T> hashSet) => hashSet.Count < 1;


		public static List<T> GetAllPublicConstantValues<T>(this Type type) {
			return type
			      .GetFields(ConstantFlags)
			      .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
			      .Select(x => (T)x.GetRawConstantValue()!)
			      .ToList();
		}
	}
}