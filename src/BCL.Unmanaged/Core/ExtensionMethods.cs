using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BCL.Unmanaged {
	/// <summary>
	///     Provides extension methods that are 'optimized' for performance. These methods utilize unsafe code.
	/// </summary>
	public static class ExtensionMethodsUnsafe {
		/// <summary>
		/// External dll that wraps System.Runtime.InteropServices and makes an internal call to
		/// CollectionsMarshal.AsSpan()
		/// </summary>
		/// <param name="list">Generic collection</param>
		/// <typeparam name="T">Generic type that will be converted to Span</typeparam>
		/// <returns></returns>
		//public static Span<T> ConvertToSpan<T>(this List<T> list) => list.AsSpan();

		/// <summary>
		/// External dll that wraps System.Runtime.InteropServices and makes an internal call to
		/// CollectionsMarshal.AsSpan()
		/// </summary>
		/// <param name="list">Generic collection</param>
		/// <typeparam name="T">Generic type that will be converted to ReadOnlySpan</typeparam>
		/// <returns></returns>
		//public static ReadOnlySpan<T> ConvertToReadOnlySpan<T>(this List<T> list) => ConvertToSpan(list);
		
		// THIS CONTEXT IS UNSAFE
		static unsafe void UnsafeMock<T>(this List<T> list) {
			var listAsSpan = list.AsSpan();
			
			// get reference to element at index '0' of listAsSpan
			ref var searchRef = ref MemoryMarshal.GetReference(listAsSpan);

			for (var i = 0; i < listAsSpan.Length; i++) {
				// Unsafe.Add is in namespace System.Runtime.CompilerServices; requires .NET 5+
				// Unsafe.Add is NOT an arithmetic operation; this method takes a referenced starting point 
				//		(we use index 0 element from our converted span) and simply adds an offset to the given
				//		managed pointer (again, 0 index of our span). 
				// The offset in this case is the INDEX OF THE LOOP 'i'
				
				//var testMockItem = Unsafe.Add(ref searchRef, i)
				
				// some other internal logic here
			}
			
			// post-process; return
		}
		
		// Same as above but without an additional conversion; requires operating on an allocated array
		static Span<T> ConvertArrayToSpan<T>(this T[] array) {
			//ref var searchRef = ref MemoryMarshal.GetArrayDataReference(array);
			return Span<T>.Empty;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveInlining)]
		static unsafe bool HasFlags<T>(T* first, T* second) where T : unmanaged, Enum {
			var pf = (byte*)first;
			var ps = (byte*)second;

			for (var i = 0; i < sizeof(T); i++)
				if ((pf[i] & ps[i]) != ps[i])
					return false;

			return true;
		}

		/// <remarks>Faster analog of Enum.HasFlag</remarks>
		/// <inheritdoc cref="Enum.HasFlag" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe bool HasFlags<T>(this T first, T second) where T : unmanaged, Enum
			=> HasFlags(&first, &second);
	}
}