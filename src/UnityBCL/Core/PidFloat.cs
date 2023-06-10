using System;
using UnityEngine;

namespace UnityBCL {
	public class PidFloat {
		Gains _gains;

		float _integrator;
		float _lastError;

		public PidFloat(Gains gains) => _gains = gains;
		public void UpdateGains(Gains gains) => _gains = gains;

		public float Process(float errorFloat, float maxError, float deltaBuffer = 1.0f, bool clamp = true) {
			var derivative = (errorFloat - _lastError) / (deltaBuffer * Time.deltaTime);

			_integrator += errorFloat * (deltaBuffer * Time.deltaTime);
			_lastError  =  errorFloat;

			var feedback = _gains.kP   * errorFloat
			               + _gains.kI * _integrator
			               + _gains.kD * derivative;

			return clamp ? Mathf.Clamp(feedback, -maxError, maxError) : feedback;
		}

		public void Reset() => _integrator = _lastError = 0;

		[Serializable]
		public struct Gains {
			public float kP;
			public float kI;
			public float kD;
		}
	}
}