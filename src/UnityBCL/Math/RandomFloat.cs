using UnityEngine;

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
		RandomizeFloatBuilder(float min, float max) {
			Min = min;
			Max = max;
		}

		RandomizeFloatBuilder() {
			Max = 0;
			Max = 0;
		}

		float Min { get; set; }
		float Max { get; set; }

		public float Get() => Random.Range(Min, Max);

		public IStartRandomFloat Build() => this;

		public IRandomFloat WithCurrentRange() => this;

		public IRandomFloat WithNewRange(float min, float max) {
			Min = min;
			Max = max;
			return this;
		}

		public static IRandomFloatBuilder InitWithLimits(float min, float max) => new RandomizeFloatBuilder(min, max);

		public static IRandomFloatBuilder InitWithoutLimits() => new RandomizeFloatBuilder();
	}
}