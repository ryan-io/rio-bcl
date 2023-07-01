using System;
using System.Threading;
using UnityEngine;

namespace UnityBCL {
	public static class Arguments {
		public class EmptyArgs {
			EmptyArgs() {
			}

			public static EmptyArgs GetNew() => new();
		}

		public class EmptyArgs<T> {
			public static EmptyArgs<T> GetNew() => new();
		}

		public readonly struct CancellationArgs {
			public CancellationArgs(CancellationToken token) => Token = token;

			public CancellationToken Token { get; }
		}

		public readonly struct GameObjectArgs {
			public GameObjectArgs(GameObject owner) => Owner = owner;

			public GameObject Owner { get; }
		}

		public readonly struct FloatArgs {
			public float Value { get; }

			public FloatArgs(float value) => Value = value;
		}

		public readonly struct PooledObjectStateChangeArgs {
			public PooledObjectStateChangeArgs(float duration, bool withCriteria, bool activationState,
				CancellationToken cancellationToken, GameObject objectToDisable, Predicate<bool>? criteria) {
				Duration          = duration;
				WithCriteria      = withCriteria;
				ActivationState   = activationState;
				CancellationToken = cancellationToken;
				ObjectToDisable   = objectToDisable;
				Criteria          = criteria;
			}

			public float Duration { get; }

			public bool WithCriteria { get; }

			public bool ActivationState { get; }

			public GameObject ObjectToDisable { get; }

			public Predicate<bool>? Criteria { get; }

			public CancellationToken CancellationToken { get; }
		}
	}
}