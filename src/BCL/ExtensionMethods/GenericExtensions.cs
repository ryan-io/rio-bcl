using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RIO.BCL {
	public static class GenericExtensions {
		/// <summary>
		///  Ensures a provided integer is within the range of a collection and greater than or equal to zero.
		/// </summary>
		/// <param name="integer">Integer to check</param>
		/// <param name="collection">Collection to compare against</param>
		/// <typeparam name="T">Generic type parameter 'T'</typeparam>
		public static void ValidateInRange<T>(this ref int integer, ICollection<T> collection) {
			if (collection.IsNullOrEmpty())
				return;

			if (integer < 0)
				integer = 0;

			else if (integer >= collection.Count)
				integer = collection.Count - 1;
		}
        
        /// <summary>
        ///  Helper method for checking if an enumerable is null or empty.
        /// </summary>
        /// <param name="enumerable">Enumerable to check</param>
        /// <typeparam name="T">Generic type T of enumerable</typeparam>
        /// <returns>True if collection is null or empty</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? enumerable)
	        => enumerable == null || !enumerable.Any();

		/// <summary>
		///  Queries a nullable CancellationToken to see if it was cancelled or if it has a value.
		/// </summary>
		/// <param name="token">Instance of CancellationToken</param>
		/// <returns>True if null or IsCancellationRequested</returns>
		public static bool WasCancelled(this CancellationToken? token) {
			return token.HasValue && token.Value.IsCancellationRequested;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public static bool WasCancelled(this CancellationToken token) {
			return token != CancellationToken.None && token.IsCancellationRequested;
		}

		/// <summary>
		///  https://briancaos.wordpress.com/2022/07/04/c-list-batch-braking-an-ienumerable-into-batches-with-net/
		///  This method will break an IEnumerable into batches of a given size. Any overflow will be returned as a final batch.
		/// </summary>
		/// <param name="enumerator">Collection to batch</param>
		/// <param name="size">Batch size</param>
		/// <typeparam name="T">Generic type</typeparam>
		/// <returns>Enumerable of an enumerable with appropriate batch sizes. The final element may not have equal size
		///  due to it being use as 'overflow'</returns>
		public static IEnumerable<IEnumerable<T>?> Batch<T>(this IEnumerable<T> enumerator, int size) {
			T[]? batch = null;
			var  count = 0;

			foreach (var item in enumerator) {
				batch ??= new T[size];

				batch[count++] = item;
				if (count != size)
					continue;

				yield return batch;

				batch = null;
				count = 0;
			}

			if (batch != null && count > 0)
				yield return batch.Take(count).ToArray();
		}
		
		public static void Empty<T>(this T?[] original) {
			var count = original.Length;

			for (var i = 0; i < count; i++) original[i] = default;
		}
	}
}