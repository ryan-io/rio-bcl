using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UnityBCL {
	[CreateAssetMenu(menuName = "Pooling/Setup New Pooled Object", fileName = "PooledObject", order = 0)]
	[Serializable]
	public class PooledObjectSetup : ScriptableObject {
		[Title("Configuration")]
		[field: SerializeField, LabelText("Pooling Asset")]
		public AssetReference Asset { get; set; }

		[ShowInInspector, ReadOnly]
		public string PoolIdentifier => Asset.AssetGUID;

		public PooledObjectSetup(AssetReference reference) {
			Asset = reference;
		}
	}
}