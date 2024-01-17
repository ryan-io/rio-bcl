// Test.Benchmark

using BenchmarkDotNet.Attributes;
using simple_plotting;

namespace Test.Benchmark.NET7;

[MemoryDiagnoser]
public class BenchmarkBmpModify {
	[Params(10, 100_000, 1_000_000)] public int Size { get; set; }

	[GlobalSetup]
	public void Setup() {
		_parsePaths = new[] { @"C:\Users\stane\Pictures\FWG_3440x1440.jpg" };
		_parser     = new BitmapParser(ref _parsePaths);
	}

	[GlobalCleanup]
	public void Cleanup() {
		_parser.Dispose();
	}

	[Benchmark]
	public void Benchmark_BitmapParser_ModifyRgbUnsafe() {
			_parser.ModifyRgbUnsafe(0, (ref int pxlIndex, ref int red, ref int green, ref int blue) => {
				                           // modify the images RGB values however you see fit
				                           if (pxlIndex % 2 == 0) {
					                           red   -= 25;
					                           green = 10;
					                           blue  += 20;
				                           }
				                           
				                           red   -= 25;
				                           green += 10;
				                           blue  += 20;
			                           });
	}

	BitmapParser _parser;
	string[]     _parsePaths;
}