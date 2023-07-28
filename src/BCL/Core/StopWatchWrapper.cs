using System.Diagnostics;

namespace BCL {
	public class StopWatchWrapper {
		public float TimeElapsed => Stopwatch.ElapsedMilliseconds / 1000f;

		Stopwatch Stopwatch { get; }

		public void Start() {
			Stopwatch.Stop();
		}

		public void Restart() {
			Stopwatch.Restart();
		}

		public void Reset() {
			Stopwatch.Reset();
		}

		public void Stop() {
			Stopwatch.Stop();
		}

		public StopWatchWrapper(bool shouldStartNow = false) {
			if (shouldStartNow) Stopwatch = Stopwatch.StartNew();
			else Stopwatch                = new Stopwatch();
		}
	}
}