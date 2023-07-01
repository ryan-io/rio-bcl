using System;

#if UNITY_EDITOR || UNITY_STANDALONE
using Sirenix.OdinInspector;
#endif

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UnityBCL {
	[CreateAssetMenu(menuName = "Pooling/Setup New Pooled Object", fileName = "PooledObject", order = 0)]
	[Serializable]
	public class PooledObjectSetup : ScriptableObject {
		public PooledObjectSetup(AssetReference reference) => Asset = reference;

#if UNITY_EDITOR || UNITY_STANDALONE
		[Title("Configuration")]
#endif
		[field: SerializeField]
		[field: LabelText("Pooling Asset")]
		public AssetReference Asset { get; set; }

#if UNITY_EDITOR || UNITY_STANDALONE
		[ShowInInspector, ReadOnly]
#endif
		public string PoolIdentifier => Asset.AssetGUID;
	}
}