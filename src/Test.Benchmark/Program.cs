﻿// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using Test.Benchmark;

BenchmarkRunner.Run<SpanBenchmarks_SimpleIntegers>();