using BCL;
using Unity.Mathematics;

namespace Procedural {
	public interface ISceneBounds {
		IObservable<int4> OnBoundaryDetermined { get; }
	}
}