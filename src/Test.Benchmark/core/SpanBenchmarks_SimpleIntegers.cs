using BenchmarkDotNet.Attributes;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using Test.Code;

namespace Test.Benchmark;

[MemoryDiagnoser]
public class SpanBenchmarks_SimpleIntegers {
	//[Params(1_000_000)] public int Size { get; set; }
	List<int> _mockList;
	int[]     _mockArray;
	int[,]    _mockArray2D;

	[GlobalSetup]
	public void SetUpMocks() {
		// var random = new Random(RandomSeed);
		// _mockList    = Enumerable.Range(1, Size).Select(_ => random.Next()).ToList();
		// _mockArray   = _mockList.ToArray();
		// _mockArray2D = new int[1000, 1000];
	}

	//[Benchmark]
	public void Bench_Span_TripleArrays_CustomStruct() {
		var span1 = SpanOwner<int>.Allocate(1000).Span;
		var span2 = SpanOwner<int>.Allocate(1000).Span;
		var span3 = SpanOwner<TestCoordRef>.Allocate(1000).Span;

		for (var i = 0; i < span1.Length; i++) {
			for (var j = 0; j < span2.Length; j++) {
				//span3[]
			}
		}
	}

	public unsafe void Bench_Span2D_SpanOwner_ForEach_Sequential_Wrapper_StackAlloc() {
		// const int TestSize = 1000;
		//
		// int*        test = stackalloc int[TestSize];
		// Span2D<int> span = new Span2D<int>(test, 3, 3, );
		//
		//
		// //var         spanWrapper = new SpanWrapper(span, spanX, spanY, spanZ);
		// //Console.WriteLine("This Span validation uses a SpanWrapper class.");
		// spanWrapper.Span[50] = 34;
		//
		// var spanCopyX = spanWrapper.Span;
		//
		// // Console.WriteLine("Performing work on spanX");
		// InternalWorkWithSpan(spanWrapper.Span);
	}

	[Benchmark]
	public unsafe void Bench_Span2D_CopyTo_WithStackAlloc() {
		const int internalSize = 200;

		int* primaryPointer = stackalloc int[internalSize * 350];
		//int* borderPointer  = stackalloc int[internalSize * internalSize];

		var primarySpan = new Span2D<int>(primaryPointer, internalSize, 350, 0);
		// var borderSpan = new Span2D<int>(
		// 	borderPointer, internalSize, internalSize, 0);

		for (var i = 0; i < primarySpan.Height; i++) {
			for (var j = 0; j < primarySpan.Width; j++) {
				InternalWorkMethod(primarySpan[i, j]);
			}
		}

		primarySpan[54, 54] = 6;
		var copySpan = 
			//InternalBenchmark_CreateCopySpan(primarySpan); 
			primarySpan.Slice(0, 0, primarySpan.Height, primarySpan.Width);   

		for (var i = 0; i < copySpan.Height; i++) {
			for (var j = 0; j < copySpan.Width; j++) {
				InternalWorkMethod(copySpan[i, j]);
			}
		}

		primarySpan[50, 50] = 22;
		copySpan[50, 50]    = 22;
		Console.WriteLine("Span[50,50]: " + primarySpan[50, 50]);
		Console.WriteLine("copySpan[50,50]: " + copySpan[50, 50]);
		Internal_SetValue(primarySpan);
		Console.WriteLine("Span[50,50]: " + primarySpan[50, 50]);
		Console.WriteLine("copySpan[50,50]: " + copySpan[50, 50]);
		copySpan[54, 54] = 7;
		
		Console.Write("Span height: " + primarySpan.Height);
		Console.Write("Span width: " + primarySpan.Width);
	}

	void Internal_SetValue(Span2D<int> span) {
		span[50, 50] = 100;
	}

	unsafe Span2D<int> InternalBenchmark_CreateCopySpan(Span2D<int> spanToCopy) {
		int* copyToAllocationPointer = stackalloc int[spanToCopy.Width * spanToCopy.Height];

		// the same 'shape' MUST be used for CopyTo to succeed (same width, height, & pitch)
		var spanToReturn = new Span2D<int>(copyToAllocationPointer, spanToCopy.Height, spanToCopy.Width, 0);

		spanToCopy.CopyTo(spanToReturn);


		return spanToReturn;
	}

	//[Benchmark()]
	public void Bench_Span2D_Creation_NoExplicitStackAlloc() {
		var span = new Span2D<int>(_mockArray2D);

		foreach (var item in span) {
			InternalWorkMethod(item);
		}
	}

	//[Benchmark()]
	public void Bench_Span2D_Creation_NoExplicitStackAlloc_TwoSpans() {
		var span1    = new Span2D<int>(_mockArray2D);
		var newArray = new int[1000, 1000];
		var span2    = new Span2D<int>(newArray);

		foreach (var item in span1) {
			InternalWorkMethod(item);
		}

		foreach (var item in span2) {
			InternalWorkMethod(item);
		}
	}

	//[Benchmark()]
	public void Bench_Span2D_2DArray() {
		var array = new int[1000, 1000];
		foreach (var item in array) {
			InternalWorkMethod(item);
		}
	}

	//[Benchmark]
	public void Bench_Span2D_SpanOwner_ForEach_Sequential_Wrapper() {
		using var buffer = SpanOwner<int>.Allocate(_mockArray.Length); //AllocateSpan();
		var       span   = buffer.Span;
		var       spanX  = buffer.Span;
		var       spanY  = buffer.Span;
		var       spanZ  = buffer.Span;

		var spanWrapper = new SpanWrapper(span, spanX, spanY, spanZ);
		//Console.WriteLine("This Span validation uses a SpanWrapper class.");
		spanWrapper.Span[50] = 34;

		//Console.WriteLine("SpanX[50]: " + spanWrapper.Span[50]);
		var spanCopyX = spanWrapper.Span;
		// Console.WriteLine("SpanCopyX[50]: " + spanCopyX[50]);

		// Console.WriteLine("Performing work on spanX");
		InternalWorkWithSpan(spanWrapper.Span);
		// Console.WriteLine("Work on spanX complete!");
		// Console.WriteLine("SpanX[50]: "            + spanWrapper.Span[50]);
		// Console.WriteLine("SpanCopyX[50]: "        + spanCopyX[50]);
		// Console.WriteLine("Testing for equality: " + (spanWrapper.Span == spanCopyX));
		// Console.WriteLine("Testing for equality: " + spanWrapper.Span.SequenceEqual(spanCopyX));
	}

	public void Bench_Span2D_SpanOwner_ForEach_Sequential_NoWrapper() {
		using var bufferX = SpanOwner<int>.Allocate(_mockArray.Length);
		var       spanX   = bufferX.Span;

		spanX[50] = 34;

		//Console.WriteLine("SpanX[50]: " + spanX[50]);
		var spanCopyX = spanX;
		// Console.WriteLine("SpanCopyX[50]: " + spanCopyX[50]);
		// Console.WriteLine("Performing work on spanX");
		InternalWorkWithSpan(spanX);
		// Console.WriteLine("Work on spanX complete!");
		// Console.WriteLine("SpanX[50]: "            + spanX[50]);
		// Console.WriteLine("SpanCopyX[50]: "        + spanCopyX[50]);
		// Console.WriteLine("Testing for equality: " + (spanX == spanCopyX));
		// Console.WriteLine("Testing for equality: " + spanX.SequenceEqual(spanCopyX));
	}

	SpanOwner<int> AllocateSpan() {
		using var bufferX = SpanOwner<int>.Allocate(_mockArray.Length);
		return bufferX;
	}

	public void Bench_Span2D_SpanOwner_ForEach_Sequential(SpanOwner<int> spanOwner) {
		var spanX = spanOwner.Span;
		spanX[50] = 34;

		Console.WriteLine("SpanX[50]: " + spanX[50]);
		var spanCopyX = spanX;
		Console.WriteLine("SpanCopyX[50]: " + spanCopyX[50]);
		InternalWorkWithSpan(spanX);
		Console.WriteLine("SpanCopyX[50]: " + spanCopyX[50]);
	}

	void InternalWorkWithSpan(Span<int> span) {
		for (var i = 0; i < span.Length; i++) {
			if (i == 50) {
				//Console.WriteLine("About to assigned index 50: " + span[i]);
			}

			span[i] = 2;
			if (i == 50) {
				//Console.WriteLine("Index 50 has been assigned to: " + span[i]);
			}
		}
	}

	public void Bench_Span2D_SpanOwner_ForEach() {
		using var bufferX = SpanOwner<int>.Allocate(BUFFER_LENGTH_X);
		using var bufferY = SpanOwner<int>.Allocate(BUFFER_LENGTH_Y);
		var       spanX   = bufferX.Span;
		var       spanY   = bufferY.Span;

		spanX[50] = 34;
		spanY[73] = 4343;

		foreach (var t in spanX) {
			InternalWorkMethod(t);
		}

		foreach (var t in spanY) {
			InternalWorkMethod(t);
		}
	}

	public void Bench_Span2D_SpanOwner_For() {
		using var bufferX = SpanOwner<int>.Allocate(BUFFER_LENGTH_X);
		using var bufferY = SpanOwner<int>.Allocate(BUFFER_LENGTH_Y);
		var       spanX   = bufferX.Span;
		var       spanY   = bufferY.Span;

		spanX[50] = 34;
		spanY[73] = 4343;

		for (var i = 0; i < spanX.Length; i++) {
			InternalWorkMethod(spanX[i]);
		}

		for (var i = 0; i < spanY.Length; i++) {
			InternalWorkMethod(spanY[i]);
		}
	}

	public void Bench_Span2D_Stackalloc_ForEach() {
		Span<int> spanX = stackalloc int[BUFFER_LENGTH_X];
		Span<int> spanY = stackalloc int[BUFFER_LENGTH_Y];

		spanX[50] = 34;
		spanY[73] = 4343;

		foreach (var t in spanX) {
			InternalWorkMethod(t);
		}

		foreach (var t in spanY) {
			InternalWorkMethod(t);
		}
	}

	public void Bench_Span2D_Stackalloc_For() {
		Span<int> spanX = stackalloc int[BUFFER_LENGTH_X];
		Span<int> spanY = stackalloc int[BUFFER_LENGTH_Y];

		spanX[50] = 34;
		spanY[73] = 4343;

		for (var i = 0; i < spanX.Length; i++) {
			InternalWorkMethod(spanX[i]);
		}

		for (var si = 0; si < spanY.Length; si++) {
			InternalWorkMethod(spanY[si]);
		}
	}

	public void Bench_Span2D() {
		var spanX = new Span<int>(new int[BUFFER_LENGTH_X]);
		var spanY = new Span<int>(new int[BUFFER_LENGTH_Y]);

		spanX[50] = 34;
		spanY[73] = 4343;

		foreach (var t in spanX) {
			InternalWorkMethod(t);
		}

		foreach (var t in spanY) {
			InternalWorkMethod(t);
		}
	}

	const int BUFFER_LENGTH_X = 1200;
	const int BUFFER_LENGTH_Y = 1000;

	public void BenchMark_MockList_WithSpan() {
		var listAsSpan = CollectionsMarshal.AsSpan(_mockList);

		for (var i = 0; i < listAsSpan.Length; i++) {
			var unit = listAsSpan[i];
			InternalWorkMethod(unit);
		}
	}

	public void BenchMark_MockList_NoSpan() {
		for (var i = 0; i < _mockList.Count; i++) {
			var unit = _mockList[i];
			InternalWorkMethod(unit);
		}
	}

	public void BenchMark_MockList_NoSpan_ForEach() {
		foreach (var unit in _mockList) {
			InternalWorkMethod(unit);
		}
	}

	public void BenchMark_MockArray_NoSpan() {
		for (var i = 0; i < _mockArray.Length; i++) {
			var unit = _mockArray[i];
			InternalWorkMethod(unit);
		}
	}

	public void BenchMark_MockArray_WithSpan() {
		var arrayAsSpan = new Span<int>();

		for (var i = 0; i < arrayAsSpan.Length; i++) {
			var unit = arrayAsSpan[i];
			InternalWorkMethod(unit);
		}
	}

	static int InternalWorkMethod(int unit) {
		return 2 * unit;
	}

	const int RandomSeed = 1000;
}