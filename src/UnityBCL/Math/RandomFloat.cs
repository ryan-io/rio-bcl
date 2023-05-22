using Random = UnityEngine.Random;

namespace UnityBCL {
	public interface IRandomFloatBuilder {
		IStartRandomFloat Build();
	}

	public interface IStartRandomFloat {
		IRandomFloat WithCurrentRange();
		IRandomFloat WithNewRange(float min, float max);
	}

	public interface IRandomFloat {
		float Get();
	}

	public class RandomizeFloatBuilder : IRandomFloatBuilder, IStartRandomFloat, IRandomFloat {
		public static IRandomFloatBuilder InitWithLimits(float min, float max) {
			return new RandomizeFloatBuilder(min, max);
		}

		public static IRandomFloatBuilder InitWithoutLimits() {
			return new RandomizeFloatBuilder();
		}

		float Min { get; set; }
		float Max { get; set; }

		public IStartRandomFloat Build() {
			return this;
		}

		public IRandomFloat WithCurrentRange() {
			return this;
		}

		public IRandomFloat WithNewRange(float min, float max) {
			Min = min;
			Max = max;
			return this;
		}

		public float Get() {
			return Random.Range(Min, Max);
		}

		RandomizeFloatBuilder(float min, float max) {
			Min = min;
			Max = max;
		}

		RandomizeFloatBuilder() {
			Max = 0;
			Max = 0;
		}
	}
}