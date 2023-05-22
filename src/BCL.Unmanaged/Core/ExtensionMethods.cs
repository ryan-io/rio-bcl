using System;
using System.Runtime.CompilerServices;

namespace BCL.Unmanaged {
	/// <summary>
	/// Provides extension methods that are 'optimized' for performance. These methods utilize unsafe code.
	/// </summary>
	public static class ExtensionMethodsUnsafe {
		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveInlining)]
		private static unsafe bool HasFlags<T>(T* first, T* second) where T : unmanaged, Enum {
			Byte* pf = (Byte*)first;
			Byte* ps = (Byte*)second;

			for (Int32 i = 0; i < sizeof(T); i++) {
				if ((pf[i] & ps[i]) != ps[i]) {
					return false;
				}
			}

			return true;
		}

		/// <remarks>Faster analog of Enum.HasFlag</remarks>
		/// <inheritdoc cref="Enum.HasFlag"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe bool HasFlags<T>(this T first, T second) where T : unmanaged, Enum {
			return HasFlags(&first, &second);
		}
	}
}