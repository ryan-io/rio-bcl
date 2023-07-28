using CommunityToolkit.HighPerformance;
using Test.Benchmark;

namespace Test.Environment;

public class TestSpan2D {
	public void TestCopyTo_WithStackAlloc() {
		var test = new SpanBenchmarks_SimpleIntegers();
		test.Bench_Span2D_CopyTo_WithStackAlloc();
	}
	
	readonly struct CoordRef {
		public int X { get; }
		public int Y { get; }

		public CoordRef(int x, int y) {
			X = x;
			Y = y;
		}
	}

	public unsafe void TestSpan2D_CopyToReference() {
		const int internalSize = 1000;

		int* primaryPointer = stackalloc int[internalSize * internalSize];
		//int* borderPointer  = stackalloc int[internalSize * internalSize];

		var primarySpan = new Span2D<int>(primaryPointer, internalSize, internalSize, 0);
		// var borderSpan = new Span2D<int>(
		// 	borderPointer, internalSize, internalSize, 0);

		for (var i = 0; i < primarySpan.Width; i++) {
			for (var j = 0; j < primarySpan.Width; j++) {
				//InternalWorkMethod(primarySpan[i, j]);
			}
		}

		var copySpan = InternalCreateCopySpan(primarySpan);
	}

	unsafe Span2D<int> InternalCreateCopySpan(Span2D<int> spanToCopy) {
		int* copyToAllocationPointer = stackalloc int[spanToCopy.Width * spanToCopy.Height];
		
		// the same 'shape' MUST be used for CopyTo to succeed (same width, height, & pitch)
		var spanToReturn = new Span2D<int>(copyToAllocationPointer, spanToCopy.Height, spanToCopy.Width, 0);

		spanToCopy.CopyTo(spanToReturn);


		return spanToReturn;
	}
	
	public unsafe void Bench_Span2D_SpanOwner_ForEach_Sequential_Wrapper_StackAlloc() {
		const int TestSize = 1000;

		int* test  = stackalloc int[TestSize * TestSize];
		//int* test2 = stackalloc int[TestSize * TestSize];

			var span  = new Span2D<int>(test,  TestSize, TestSize, 0);
		for (var i = 0; i < 3; i++) {
			//var span2 = new Span2D<int>(test2, TestSize, TestSize, i);
			span[1, 2] = 3;
			// var spanOwner = SpanOwner<CoordRef>.Allocate(TestSize);
			// Console.WriteLine(span.Length);
			// Console.WriteLine(span2.Length);
			// Console.WriteLine(spanOwner.Span.Length);
		}
	}
}