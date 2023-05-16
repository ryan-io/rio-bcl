using System.Collections;

namespace OmniBCL.ExtensionMethods; 

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
}