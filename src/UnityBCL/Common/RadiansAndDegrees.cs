using Sirenix.OdinInspector;
using UnityEngine;

namespace UnityBCL {
	public class RadiansAndDegrees : MonoBehaviour {
		[ShowInInspector] [ReadOnly] float _angleInDegrees;

		[ShowInInspector] [ReadOnly] float _angleInRadians;

		[ShowInInspector] [ReadOnly] float _cosineOfAngle;
		[ShowInInspector] [ReadOnly] float _dotProductValue;
		[ShowInInspector] [ReadOnly] float _sineOfAngle;

		[Button]
		[PropertySpace(20, 20)]
		void ConvertAngleToDegrees(float angleInRadians)
			=> _angleInDegrees = Mathf.Rad2Deg * angleInRadians;

		[Button]
		[PropertySpace(20, 20)]
		void ConvertAngleToRadians(float angleInDegrees)
			=> _angleInRadians = Mathf.Deg2Rad * angleInDegrees;

		[Button]
		[PropertySpace(20, 20)]
		void CosineOfAngle(float angleInDegrees)
			=> _cosineOfAngle = Mathf.Cos(angleInDegrees * Mathf.Deg2Rad);

		[Button]
		[PropertySpace(20, 20)]
		void SineOfAngle(float angleInDegrees)
			=> _sineOfAngle = Mathf.Sin(angleInDegrees * Mathf.Deg2Rad);

		[Button]
		[PropertySpace(20, 20)]
		void CalculateDotProduct(Vector2 v1, Vector2 v2)
			=> _dotProductValue = Vector3.Dot(v1.normalized, v2.normalized);
	}
}