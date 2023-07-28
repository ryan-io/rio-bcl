using System;
using CommunityToolkit.HighPerformance.Buffers;

namespace Test.Code {
	public class LogMarshalSizeOf {
		public void WriteSizeOfMarshalObjectPropertiesToConsole(MarshalObject marshalObject) {
			// Span<int> xAlloc = stackalloc int[marshalObject.CoreMap.GetLength(0)];
			// Span<int> yAlloc = stackalloc int[marshalObject.CoreMap.GetLength(1)];

			var bufferX = Test(marshalObject);
			using var   bufferY = SpanOwner<int>.Allocate(marshalObject.CoreMap.GetLength(1));
			var         spanX   = bufferX.Span;
			var         spanY   = bufferY.Span;


			Console.WriteLine("Validating xAlloc length: " + spanX.Length);
			Console.WriteLine("Validating yAlloc length: " + spanY.Length);

			spanX[50]     = 34;
			spanY[73] = 4343;
			Console.WriteLine(spanX[12]);
			Console.WriteLine(spanX[3]);
			Console.WriteLine(spanY[23]);
			Console.WriteLine(spanY[24]);
			
		}

		static SpanOwner<int> Test(MarshalObject marshalObject) {
			using var bufferX = SpanOwner<int>.Allocate(marshalObject.CoreMap.GetLength(0));
			return bufferX;
		}
	}
}