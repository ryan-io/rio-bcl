using System;
using System.Collections.Generic;
using BCL;
using Sirenix.OdinInspector;
using UnityBCL.Serialization;
using UnityBCL.Serialization.Core;
using UnityEngine;

namespace Curves {
	[HideMonoScript]
	[ExecuteInEditMode]
	public abstract class Polynomial : MonoBehaviour, IColor {
		protected bool _drawPoints;

		public abstract    SplineType SplineType    { get; }
		protected abstract Vector2[]  DefaultPoints { get; }

		public bool DrawHandleLines => _drawHandleLines == Toggle.Yes;

		public IReadOnlyList<Vector2> PointsReadonly => _points;

		public IReadOnlyList<Vector2> PointsExtractedReadonly {
			get {
				ExtractPoints();
				return _extractedPoints;
			}
		}

		public bool ShouldDrawRootHandles => _drawRootHandles == Toggle.Yes;

		public bool ShouldDrawLabels => _drawLabels == Toggle.Yes;

		protected bool CantSave => _extractedPoints == null || _extractedPoints.Length < 1;

		protected          bool DoNotUpdate => _autoUpdate == Toggle.No;
		protected abstract void Reset();

		void Update() {
			if (DoNotUpdate)
				return;

			ExtractPoints();
		}

		public             Color SerializedColor => _color;
		protected abstract void  ExtractPoints();

		public virtual Vector2 GetPoint(int index) {
			if (index < 0) {
				Debug.LogWarning("Index is less than '0'. The first point in the array will be returned.");
				index = 0;
			}

			return _points[index];
		}

		public virtual void SetPoint(int index, Vector2 point) {
			if (index < 0)
				return;

			_points[index] = point;
		}

#region Plumbing

		[TitleGroup("Settings", "Various settings to control various spline gizmos", Indent = true)]
		[LabelText("Reset Root Position")]
		[LabelWidth(200)]
		[Indent]
		[HorizontalGroup("Settings/Root")]
		[Button]
		[PropertyOrder(-5)]
		[PropertySpace(0, 10)]
		[GUIColor(80 / 255f, 210 / 255f, 180 / 255f)]
		void ResetRootPosition()
			=> transform.position = Vector3.zero;

		[TitleGroup("Settings", "Various settings to control various spline gizmos", Indent = true)]
		[LabelText("Reset Root Rotation")]
		[LabelWidth(200)]
		[Indent]
		[HorizontalGroup("Settings/Root")]
		[Button]
		[PropertyOrder(-5)]
		[PropertySpace(0, 10)]
		[GUIColor(141 / 255f, 61 / 255f, 175 / 255f)]
		void ResetRootRotation()
			=> transform.rotation = Quaternion.identity;

		[TitleGroup("Settings", "Various settings to control various spline gizmos", Indent = true)]
		[LabelText("Reset Root Scale")]
		[LabelWidth(200)]
		[Indent]
		[HorizontalGroup("Settings/Root")]
		[Button]
		[PropertyOrder(-5)]
		[PropertySpace(0, 10)]
		[GUIColor(84 / 255f, 195 / 255f, 255 / 255f)]
		void ResetRootScale()
			=> transform.localScale = Vector3.one;

		[TitleGroup("Settings", "Various settings to control various spline gizmos", Indent = true)]
		[LabelText("Root Handles")]
		[LabelWidth(200)]
		[Indent]
		[EnumToggleButtons]
		[PropertyOrder(-10)]
		[PropertySpace(0, 10)]
		[SerializeField]
		protected Toggle _drawRootHandles = Toggle.No;

		[VerticalGroup("Settings/Vertical", AnimateVisibility = true, PaddingBottom = 5f, PaddingTop = 5f)]
		[LabelText("Labels")]
		[LabelWidth(200)]
		[Indent]
		[EnumToggleButtons]
		[PropertyOrder(-20)]
		[SerializeField]
		protected Toggle _drawLabels = Toggle.Yes;

		[VerticalGroup("Settings/Vertical", AnimateVisibility = true, PaddingBottom = 5f, PaddingTop = 5f)]
		[SerializeField]
		[LabelText("Name")]
		[LabelWidth(200)]
		[Indent]
		[PropertyOrder(-100)]
		[PropertySpace(0, 10)]
		protected string _splineName = "New Spline";

		[TitleGroup("Colors", "Pre-defined color palettes may be used or you can create a " +
		                      "custom palette (or color) to use with your line tool", Indent = true)]
		[SerializeField]
		[PropertyOrder(50)]
		[ColorPalette]
		[Indent]
		[LabelWidth(200)]
		[LabelText("Spline Color")]
		protected Color _color = Color.white;

		[SerializeField]
		[TitleGroup("Colors")]
		[PropertyOrder(50)]
		[PropertySpace(10, 20)]
		[ColorPalette]
		[Indent]
		[LabelWidth(200)]
		[LabelText("Points Color")]
		protected Color _pointColor = Color.yellow;

		[TitleGroup("Polynomial Shape", "Control of lines, curves, & splines", Indent = true)]
		[LabelText("Points", true)]
		[PropertyOrder(-10)]
		[LabelWidth(200)]
		[Indent]
		[PropertySpace(10, 0)]
		[ListDrawerSettings(Expanded = true,      DraggableItems = true, ShowIndexLabels = true, NumberOfItemsPerPage =
			10,             HideAddButton = true, HideRemoveButton = true)]
		[SerializeField]
		protected Vector2[] _points;

		[TitleGroup("Point Extraction", "Control of lines, curves, & splines", Indent = true)]
		[ListDrawerSettings(Expanded = true, DraggableItems = true, ShowIndexLabels = true, NumberOfItemsPerPage = 10)]
		[Indent]
		[LabelWidth(200)]
		[PropertyOrder(-9)]
		[PropertySpace(10)]
		[SerializeField]
		[EnumToggleButtons]
		[PropertyTooltip("If toggled, the extracted points will be continuously updated when changed. If this is not " +
		                 "set to auto update, you will nee dto manually press the 'Extract' button to recalculate "    +
		                 "changed points.")]
		protected Toggle _autoUpdate = Toggle.No;

		[TitleGroup("Point Extraction", "Control of lines, curves, & splines", Indent = true)]
		[LabelText("Steps", true)]
		[PropertyOrder(-10)]
		[LabelWidth(200)]
		[Indent]
		[PropertySpace(10, 0)]
		[Range(2, 10000)]
		public int _steps = 10;

		[HorizontalGroup("Point Extraction/SplineDisplays")]
		[LabelWidth(200)]
		[Indent]
		[PropertySpace(10, 0)]
		[GUIColor(80 / 255f, 210 / 255f, 180 / 255f)]
		[Button("Save", ButtonSizes.Medium, ButtonStyle.CompactBox)]
		[EnableIf("@!CantSave")]
		protected void SavePoints() {
			if (CantSave) {
				Utility.Log.LogNoPointsWarning();
				return;
			}

			var data = ScriptableObject.CreateInstance<ExtractedPointsSO>();
			data.Initialize(_extractedPoints, SplineType);

			var assetSaver =
				new GenericSaver(
					Utility.Strings.ParentFolder + Utility.Strings.Slash + Utility.Strings.SaveToFolder);
			assetSaver.Save(data, _splineName);
		}

		[TitleGroup("Point Extraction", "Control of lines, curves, & splines", Indent = true)]
		[ListDrawerSettings(Expanded = true, DraggableItems = true, ShowIndexLabels = true, NumberOfItemsPerPage = 10,
			HideAddButton = true, HideRemoveButton = true)]
		[Indent]
		[ReadOnly]
		[PropertyOrder(10)]
		[SerializeField]
		protected Vector2[] _extractedPoints;

		[TitleGroup("Point Extraction", "Control of lines, curves, & splines", Indent = true)]
		[SerializeField]
		[Range(0.2f, 1000f)]
		[Indent]
		[PropertySpace(10)]
		[LabelWidth(200)]
		float _pointRadius = 0.35f;

		[TitleGroup("Point Extraction", "Control of lines, curves, & splines", Indent = true)]
		[HorizontalGroup("Point Extraction/SplineDisplays")]
		[LabelWidth(150)]
		[Indent]
		[PropertySpace(10, 0)]
		[GUIColor(141 / 255f, 61 / 255f, 175 / 255f)]
		[Button("Toggle Points", ButtonSizes.Medium, ButtonStyle.CompactBox)]
		void ToggleDrawPoints()
			=> _drawPoints = !_drawPoints;

		[TitleGroup("Point Extraction", "Control of lines, curves, & splines", Indent = true)]
		[HorizontalGroup("Point Extraction/SplineDisplays")]
		[LabelWidth(150)]
		[Indent]
		[PropertySpace(10, 20)]
		[GUIColor(254 / 255f, 214 / 255f, 82 / 255f)]
		[DisableIf("@!DoNotUpdate")]
		[Button("Reset", ButtonSizes.Medium, ButtonStyle.CompactBox)]
		protected void ResetIteratedPoints() => _extractedPoints = Array.Empty<Vector2>();

		[VerticalGroup("Settings/Vertical", AnimateVisibility = true, PaddingBottom = 5f, PaddingTop = 5f)]
		[LabelText("Tangent Lines", true)]
		[LabelWidth(200)]
		[PropertyOrder(-10)]
		[Indent]
		[PropertySpace(10)]
		[EnumToggleButtons]
		[SerializeField]
		Toggle _drawHandleLines = Toggle.Yes;

		void ValidateSteps() {
			if (_steps % 2 != 0)
				_steps = Mathf.RoundToInt(_steps + 1);
		}

		protected void OnDrawGizmosSelected() {
			if (CantSave)
				return;

			DrawIteratedPoints();
		}

		protected void DrawIteratedPoints() {
			if (!_drawPoints)
				return;

			for (var i = 0; i < _extractedPoints.Length; i++)
				DrawPoints(i);
		}

		protected void DrawPoints(int i) {
			Gizmos.color = _pointColor;
			Gizmos.DrawSphere(_extractedPoints[i], _pointRadius);
		}

#endregion
	}
}