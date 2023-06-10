using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UnityBCL.Editor {
	public static class ProjectLayersAndTags {
		const int MaxLayers = 31;

		[MenuItem("Project Management/Layers and Tags/Populate Tags")]
		static void PopulateProjectTags() {
			var tags = new List<string>(InternalEditorUtility.tags);
			AddTag(tags, "LevelBounds");
			AddTag(tags, "Fx");
			AddTag(tags, "Enemy");
			AddTag(tags, "World");
			AddTag(tags, "NoMask");
			AddTag(tags, "LevelGenerator");
			AddTag(tags, "Model");
			AddTag(tags, "Missile");
			AddTag(tags, "Weapon");
			AddTag(tags, "Projectile");
			AddTag(tags, "Awareness");
			AddTag(tags, "PatrolNode");
			AddTag(tags, "LabelDebug");
			AddTag(tags, "Obstacle");
			AddTag(tags, "Sensors");
			AddTag(tags, "UICamera");
			AddTag(tags, "EnemyProjectile");
			AddTag(tags, "PlayerProjectile");
			AddTag(tags, "Pathfinding");
			AddTag(tags, "Player");

			Debug.Log("Project tags have been populated.");
		}

		[MenuItem("Project Management/Layers and Tags/Populate Game Object Layers")]
		static void PopulateGameObjectLayers() {
			AddLayer("PlayerCamera");
			AddLayer("CutsceneCamera");
			AddLayer("UICamera");
			AddLayer("ExternalCamera");
			AddLayer("UIParticles");
			AddLayer("Ground");
			AddLayer("Obstacles");
			AddLayer("ObstaclesDoors");
			AddLayer("MovingPlatform");
			AddLayer("Hole");
			AddLayer("NoPathfinding");
			AddLayer("Awareness");
			AddLayer("Projectile");
			AddLayer("DamageInflictor");
			AddLayer("Sensors");
			AddLayer("Hostile");
			AddLayer("Debug");
			AddLayerAtIndex("Player", 31);
			Debug.Log("Project game object layers have been populated.");
		}

		[MenuItem("Project Management/Layers and Tags/Populate Sorting Layers")]
		static void PopulateSortingLayers() {
			AddSortingLayer("Ground");
			AddSortingLayer("Light");
			AddSortingLayer("Background");
			AddSortingLayer("Obstacles");
			AddSortingLayer("Foreground - 1");
			AddSortingLayer("Foreground - 2");
			AddSortingLayer("Foreground - 3");
			AddSortingLayer("Characters");
			AddSortingLayer("Y-Sorting");
			AddSortingLayer("Above");

			Debug.Log("Project sorting layers have been populated.");
		}

		static void AddSortingLayer(string layerName) {
			var serializedObject =
				new SerializedObject(AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset"));

			var sortingLayers = serializedObject.FindProperty("m_SortingLayers");

			for (var i = 0; i < sortingLayers.arraySize; i++)
				if (sortingLayers.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue.Equals(layerName))
					return;

			sortingLayers.InsertArrayElementAtIndex(sortingLayers.arraySize);
			var newLayer = sortingLayers.GetArrayElementAtIndex(sortingLayers.arraySize - 1);
			newLayer.FindPropertyRelative("name").stringValue  = layerName;
			newLayer.FindPropertyRelative("uniqueID").intValue = layerName.GetHashCode();
			serializedObject.ApplyModifiedProperties();
		}

		static bool AddLayer(string layerName) {
			var tagManager =
				new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

			var layersProp = tagManager.FindProperty("layers");

			if (!PropertyExists(layersProp, 0, MaxLayers, layerName))
				for (int i = 8, j = MaxLayers; i < j; i++) {
					var sp = layersProp.GetArrayElementAtIndex(i);
					if (sp.stringValue == "") {
						// Assign string value to layer
						sp.stringValue = layerName;
						Debug.Log("Layer: " + layerName + " has been added");
						// Save settings
						tagManager.ApplyModifiedProperties();
						return true;
					}
				}
			else
				Debug.Log("Layer: " + layerName + " already exists");

			return false;
		}

		static bool AddLayerAtIndex(string layerName, int index) {
			if (index > 31 || index < 1) {
				Debug.LogWarning("Index was out of bounds. Please correct this and try to add layer again.");
				return false;
			}

			var tagManager =
				new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

			var layersProp = tagManager.FindProperty("layers");

			if (!PropertyExists(layersProp, 0, MaxLayers, layerName)) {
				//for (int i = 8, j = MaxLayers; i < j; i++) {
				var sp = layersProp.GetArrayElementAtIndex(index);
				if (sp.stringValue == "") {
					// Assign string value to layer
					sp.stringValue = layerName;
					Debug.Log("Layer: " + layerName + " has been added");
					// Save settings
					tagManager.ApplyModifiedProperties();
					return true;
				}
				//}
			}
			else {
				Debug.Log("Layer: " + layerName + " already exists");
			}

			return false;
		}


		static bool LayerExists(string layerName) {
			var tagManager =
				new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

			var layersProp = tagManager.FindProperty("layers");
			return PropertyExists(layersProp, 0, MaxLayers, layerName);
		}

		static bool PropertyExists(SerializedProperty property, int start, int end, string value) {
			for (var i = start; i < end; i++) {
				var t = property.GetArrayElementAtIndex(i);

				if (t.stringValue.Equals(value))
					return true;
			}

			return false;
		}

		static void AddTag(IList<string> tags, string tag) {
			if (tags == null || string.IsNullOrEmpty(tag))
				return;

			for (int t = 0, count = tags.Count; t < count; ++t)
				if (tag.Equals(tags[t], StringComparison.Ordinal))
					return;

			InternalEditorUtility.AddTag(tag);
			tags.Add(tag);
		}
	}
}