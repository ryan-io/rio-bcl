#if UNITY_EDITOR || UNITY_STANDALONE

using Sirenix.OdinInspector;
using UnityEngine;

namespace UnityBCL.Editor {
	public class ObjectSearching : MonoBehaviour {
		[ShowInInspector] [BoxGroup("0", false)] [Indent]
		SearchByLayer _searchByLayer = new SearchByLayer();

		[ShowInInspector] [BoxGroup("0", false)] [Indent]
		SearchByTag _searchByTag = new SearchByTag();
	}
}

#endif