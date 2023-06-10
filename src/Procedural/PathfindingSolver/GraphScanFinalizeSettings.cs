using System;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Procedural {
	[Serializable]
	public class GraphScanFinalizeSettings {
		public GraphScanFinalizeSettings(ColliderType colliderType, float diameter, float height, LayerMask mask) {
			CollisionType              = colliderType;
			CollisionDetectionDiameter = diameter;
			CollisionDetectionHeight   = height;
			HeightTestLayerMask        = mask;
		}

		[Title("On Scan Complete Settings")]
		[field: SerializeField]
		public ColliderType CollisionType { get; private set; }

		[field: SerializeField]
		[field: Range(0.05f, 5.0f)]
		public float CollisionDetectionDiameter { get; private set; }

		[field: SerializeField]
		[field: Range(0.5f, 100.0f)]
		[field: ShowIf("@CollisionType == ColliderType.Capsule")]
		public float CollisionDetectionHeight { get; private set; }

		[field: SerializeField] public LayerMask ObstacleLayerMask { get; private set; }

		[field: SerializeField] public LayerMask HeightTestLayerMask { get; private set; }

		public static GraphScanFinalizeSettings Default() => new(ColliderType.Capsule, 0.1f, 1.0f, 0);
	}
}