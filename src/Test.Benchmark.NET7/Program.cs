// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using Test.Benchmark;

Console.WriteLine("Hello, World!");

BenchmarkRunner.Run<BenchmarkBmpModify>();
