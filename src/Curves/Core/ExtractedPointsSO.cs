using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Curves {
	[HideMonoScript]
	[InlineEditor(InlineEditorObjectFieldModes.Boxed)]
	public class ExtractedPointsSO : ScriptableObject {
		[ShowInInspector] [ReadOnly] [SerializeField]
		SplineType _splineType = SplineType.None;

		[ShowInInspector]
		[ReadOnly]
		[SerializeField]
		[TableList(HideToolbar = true, ShowIndexLabels = true, NumberOfItemsPerPage = 15,
			CellPadding = 5)]
		[ListDrawerSettings(ShowPaging = true, HideAddButton = true, HideRemoveButton = true)]
		Vector2[] _points;

		public SplineType SplineType => _splineType;

		public Vector2[] Load() {
			try {
				return _points;
			}

			catch (EmptyArrayException e) {
				Utility.Log.Error(e.Message);
				throw;
			}

			catch (NullReferenceException e) {
				Utility.Log.Error(e.Message);
				throw;
			}
		}

		public void Initialize(Vector2[] points, SplineType splineType) {
			try {
				_points     = points;
				_splineType = splineType;
			}

			catch (EmptyArrayException e) {
				Utility.Log.Error(e.Message);
				throw;
			}

			catch (NullReferenceException e) {
				Utility.Log.Error(e.Message);
				throw;
			}
		}
	}
}