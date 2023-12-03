using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using riolog;

namespace Test.Environment.core;

public class TwelvePointTwo_AsyncLocks {
	public int Integer { get; set; } = 0;
	Random     Random  { get; }      = new();

	public void Test(int id) {
		try {
			_semaphore.Wait();
			_logger.LogInformation("Semaphore wait complete: {Id}", id);
			GeneratedAndAdd();
		}
		catch (Exception e) {
			_logger.LogError("{Error}", e.Message);
			throw;
		}
		finally {
			_semaphore.Release();
		}
	}

	/// <summary>
	///  This is the most appropriate solution to an async lock.
	/// </summary>
	/// <param name="id">Integer for thread id</param>
	public async Task TestAsync(int id) {
		try {
			await _semaphore.WaitAsync();
			_logger.LogInformation("Semaphore has been awaited: {Id}", id);
			await GeneratedAndAdd();
		}
		catch (Exception e) {
			_logger.LogError("{Error}", e.Message);
			throw;
		}
		finally {
			_semaphore.Release();
		}
	}

	public async Task TestAsyncNoSemaphore(int id) {
		try {
			await Task.Yield();
			_logger.LogInformation("Semaphore has been awaited: {Id}", id);
			await GeneratedAndAdd();
		}
		catch (Exception e) {
			_logger.LogError("{Error}", e.Message);
			throw;
		}
	}

	async Task GeneratedAndAdd() {
		await Task.Delay(TimeSpan.FromMilliseconds(50));
		Integer += Random.Next(0, 50);
	}

	public async Task GenerateAndAddToBagNoSemaphore() {
		Task[] tasks = new Task[10];

		for (var i = 0; i < 10; i++) {
			tasks[i] = Task.Run(async () => {
				                    await Task.Delay(TimeSpan.FromMilliseconds(60));
				                    _bag.Add(Random.Next(0, 50));
			                    });
		}

		await Task.WhenAll(tasks);
		_logger.LogInformation("Output: ");
		foreach (var integer in _bag) {
			_logger.LogInformation("{Integer}", integer);
		}
	}

	readonly SemaphoreSlim      _semaphore = new(1, 5);
	readonly ILogger            _logger    = InternalLogFactory.SetupAndStartAsLogger(Output.Console);
	readonly ConcurrentBag<int> _bag       = new();
}