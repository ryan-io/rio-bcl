using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Curves {
	[HideMonoScript]
	public class PolyMonoHook : MonoBehaviour, IPolyMonoHook {
		const string NoDataSO = "There is no 'ExtractPoints' scriptableobject assigned to the 'Data' field. Please" +
		                        " assign an SO and try again.";

		const string NoExtractPoints = "There is no vectory array to extact points from. Please check to verify " +
		                               "that the points array was properly extract.";

		[TitleGroup("Settings")]
		[SerializeField]
		[LabelText("Draw Polynomial Points")]
		[EnumToggleButtons]
		[Indent]
		[PropertyOrder(10)]
		[PropertySpace(10, 10)]
		Toggle _drawPolyPoints = Toggle.Yes;

		[TitleGroup("Settings")]
		[ShowInInspector]
		[PropertyOrder(10)]
		[PropertySpace(10, 10)]
		[Indent]
		[SerializeField]
		[Range(0.05f, 25f)]
		float _radius = 5;

		[TitleGroup("Settings")]
		[SerializeField]
		[PropertyOrder(50)]
		[ColorPalette]
		[Indent]
		[LabelWidth(200)]
		[LabelText("Draw Color")]
		Color _color = Color.white;

		[TitleGroup("Processed Points")]
		[ShowInInspector]
		[ReadOnly]
		[PropertyOrder(10)]
		[PropertySpace(10, 10)]
		[Indent]
		[TableList(AlwaysExpanded = true, HideToolbar = true, ShowIndexLabels = true, NumberOfItemsPerPage = 15,
			CellPadding = 5)]
		[SerializeField]
		Vector2[] _processedPoints;

		[SerializeField] [InlineEditor(InlineEditorObjectFieldModes.Foldout)] [HideLabel] [OnValueChanged("Reset")]
		ExtractedPointsSO _data;

		public SplineType SplineType {
			get {
				if (_data == null)
					return SplineType.None;

				return _data.SplineType;
			}
		}

		public Color Color
			=> _color;

		public bool ShouldDraw
			=> _drawPolyPoints == Toggle.Yes;

		bool GuardAgainstNoLocalSpacePoints
			=> _processedPoints == null || _processedPoints.Length < 1;

		bool GuardAgainstNoSO
			=> _data == null;

		[Button(ButtonSizes.Large, ButtonStyle.CompactBox)]
		[GUIColor(254 / 255f, 214 / 255f, 82 / 255f)]
		[PropertySpace(15)]
		[HorizontalGroup("Data - Processed Points/Points")]
		[Indent]
		[InfoBox("Reset the points to the original extracted points")]
		void Reset() {
			if (GuardAgainstNoSO) {
				Utility.Log.Warning(NoDataSO);
				_processedPoints = null;

				return;
			}

			_processedPoints = _data.Load();
		}

#if UNITY_EDITOR || UNITY_STANDALONE

		void OnDrawGizmosSelected() {
			if (!ShouldDraw || _processedPoints == null)
				return;

			var count = _processedPoints.Length;

			Gizmos.color = _color;

			for (var i = 0; i < count; i++) Gizmos.DrawSphere(_processedPoints[i], _radius);
		}

#endif
		public Vector2[] GetPoints()
			=> _processedPoints;

		[TitleGroup("Data - Processed Points")]
		[Button(ButtonSizes.Large, ButtonStyle.CompactBox)]
		[GUIColor(80 / 255f, 210 / 255f, 180 / 255f)]
		[HorizontalGroup("Data - Processed Points/Points")]
		[Indent]
		[PropertySpace(15)]
		[InfoBox(
			"This button will generate a new array from the defined points where the points are converted to the " +
			"local space of this game object")]
		void ConvertPointsToLocalSpace() {
			if (GuardAgainstNoSO) {
				Utility.Log.Warning(NoDataSO);
				return;
			}

			var points = _data.Load();

			if (GuardAgainstExtractPointsNullOrEmpty(points)) {
				Utility.Log.Warning(NoExtractPoints);

				return;
			}

			_processedPoints = new Vector2[points.Length];

			for (var i = 0; i < points.Length; i++)
				ConvertPointToLocalSpace(i, points);
		}

		bool GuardAgainstExtractPointsNullOrEmpty(IReadOnlyCollection<Vector2> points)
			=> points == null || points.Count < 1;

		void ConvertPointToLocalSpace(int i, Vector2[] points)
			=> _processedPoints[i] = transform.InverseTransformPoint(points[i]);
	}
}