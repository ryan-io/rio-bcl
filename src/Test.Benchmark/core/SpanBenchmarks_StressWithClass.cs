// Test.Benchmark

using BenchmarkDotNet.Attributes;

namespace Test.Benchmark;

[MemoryDiagnoser]
public class SpanBenchmarks_StressWithClass {
	[Params(10, 100_000, 10_000_000)] public int Size { get; set; }
	List<BenchMarkStress>                        _mockList;
	BenchMarkStress[]                            _mockArray;

	[GlobalSetup]
	public void SetUpMocks() {
		_mockList  = Enumerable.Range(1, Size).Select(_ => new BenchMarkStress()).ToList();
		_mockArray = _mockList.ToArray();
	}

	[Benchmark]
	public void BenchMark_MockList_WithSpan() {
		var listAsSpan = new Span<BenchMarkStress>(_mockArray);

		for (var i = 0; i < listAsSpan.Length; i++) {
			var unit = listAsSpan[i];
			InternalWorkMethod(unit);
		}
	}

	[Benchmark]
	public void BenchMark_MockList_NoSpan() {
		for (var i = 0; i < _mockList.Count; i++) {
			var unit = _mockList[i];
			InternalWorkMethod(unit);
		}
	}

	[Benchmark]
	public void BenchMark_MockList_NoSpan_ForEach() {
		foreach (var unit in _mockList) {
			InternalWorkMethod(unit);
		}
	}

	[Benchmark]
	public void BenchMark_MockArray_NoSpan() {
		for (var i = 0; i < _mockArray.Length; i++) {
			var unit = _mockArray[i];
			InternalWorkMethod(unit);
		}
	}

	[Benchmark]
	public void BenchMark_MockArray_WithSpan() {
		var arrayAsSpan = new Span<BenchMarkStress>(_mockArray);

		for (var i = 0; i < arrayAsSpan.Length; i++) {
			var unit = arrayAsSpan[i];
			InternalWorkMethod(unit);
		}
	}

	static int InternalWorkMethod(BenchMarkStress unit) {
		return 2 * unit.StressValue;
	}

	const int RandomSeed = 1000;
}