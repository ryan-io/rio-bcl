// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using Test.Benchmark;
using Test.Benchmark.NET7;

Console.WriteLine("Hello, World!");

BenchmarkRunner.Run<BenchmarkBmpModify>();
