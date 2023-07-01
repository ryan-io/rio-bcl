#if UNITY_EDITOR || UNITY_STANDALONE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
#if UNITY_STANDALONE || UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityBCL.Editor {
	[Serializable]
	public class SearchByTag {
		[ShowInInspector]
		[BoxGroup("0", false)]
		[Indent]
		[SceneObjectsOnly]
		[ListDrawerSettings(HideAddButton = true, HideRemoveButton = true)]
		[Searchable]
		List<GameObject> _objectsFound = null!;

		[ShowInInspector] [ValueDropdown("TagsDropDown")] [BoxGroup("0", false)] [Indent]
		string _tag = string.Empty;

		static IEnumerable TagsDropDown => InternalEditorUtility.tags.AsEnumerable();

		[Button(ButtonSizes.Medium, ButtonStyle.CompactBox)]
		[BoxGroup("0", false)]
		[Indent]
		void Search() {
			_objectsFound?.Clear();

			var objects = Object.FindObjectsOfType<GameObject>();
			var objsToSelect = objects
			   .Where(o => o.CompareTag(_tag));

			_objectsFound = objsToSelect.ToList();
		}
	}

	[Serializable]
	public class SearchByLayer {
		[SerializeField] [BoxGroup("0", false)] [Indent]
		LayerMask _layersToSearchFor = 0;

		[ShowInInspector]
		[BoxGroup("0", false)]
		[Indent]
		[SceneObjectsOnly]
		[ListDrawerSettings(HideAddButton = true, HideRemoveButton = true)]
		[Searchable]
		List<GameObject> _objectsFound = null!;

		[Button(ButtonSizes.Medium, ButtonStyle.CompactBox)]
		[BoxGroup("0", false)]
		[Indent]
		void Search() {
			_objectsFound?.Clear();

			var objects = Object.FindObjectsOfType<GameObject>();
			var objsToSelect = objects
			   .Where(o => _layersToSearchFor.ContainsMask(o.layer));

			_objectsFound = objsToSelect.ToList();
		}
	}
}

#endif