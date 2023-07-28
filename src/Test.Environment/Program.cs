// See https://aka.ms/new-console-template for more information

using Test.Benchmark;
using Test.Environment;

Console.WriteLine("Hello, World!");

var tests = new TestSpan2D();
tests.TestCopyTo_WithStackAlloc();