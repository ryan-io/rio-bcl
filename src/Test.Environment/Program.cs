using System.Diagnostics;
using Test.Environment.core;

var sw      = Stopwatch.StartNew();
var semTest = new TwelvePointTwo_AsyncLocks();

try {
//	await semTest.TestAsync(1);
	
	var task = TestAsync(semTest);
	await task; // this does NOT deadlock

	//TestSynchronous(semTest); // this will deadlock
	
	//TestSynchronousWithTaskCreation(semTest); // this will deadlock
	
	Console.WriteLine($"Integer: {semTest.Integer}");
	Console.WriteLine("All tasks completed");
}
catch (Exception e) {
	Console.WriteLine(e);
}

sw.Stop();
Console.WriteLine($"Elapsed: {sw.Elapsed}");

return 0;

async Task TestAsync(TwelvePointTwo_AsyncLocks semaphoreSlimTry) {
	var tasks = new List<Task>();
	for (var i = 0; i < 100; i++) {
		var thread = i;
		tasks.Add(Task.Run(async () => await semaphoreSlimTry.TestAsync(thread)));
	}
	await Task.WhenAll(tasks);
}

void TestSynchronous(TwelvePointTwo_AsyncLocks semaphoreSlimTry) {
	for (var i = 0; i < 10; i++) {
		var thread = i;
		semaphoreSlimTry.Test(thread);
	}
}

void TestSynchronousWithTaskCreation(TwelvePointTwo_AsyncLocks semaphoreSlimTry) {
	for (var i = 0; i < 10; i++) {
		var thread = i;
		Task.Run(async () => await semaphoreSlimTry.TestAsync(thread));
	}
}