using System;
using System.Collections.ObjectModel;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Curves {
	[HideMonoScript]
	[ExecuteInEditMode]
	public class Spline2D : Polynomial {
		int _currentConnectionCount = 1;

		int _currentPointCount = 4;

		public             int PointCount => _points.Length;
		public override    SplineType SplineType => SplineType.Spline;
		protected override Vector2[] DefaultPoints => Utility.Defaults.GetDefaultPoints(SplineType.Spline);
		public             int LastPointIndex => _points.Length - 1;
		public             Vector2 FirstPoint => _points[0];
		public             Vector2 LastPoint => _points[^1];
		public             int ConnectionCount => _connections.Length;
		public             int LastConnectionIndex => _connections.Length - 1;
		public             Utility.Spline.Connection FirstConnection => _connections[0];
		public             Utility.Spline.Connection LastConnection => _connections[^1];
		public             ReadOnlyCollection<Vector2> PointsReadOnly => Array.AsReadOnly(_points);
		public             int CurveCount => (_points.Length - 1) / 3;
		public             bool Loop => _loopSpline == Toggle.Yes;

		void OnValidate() {
			ValidatePointArraySize();
			ValidateConnectionArraySize();
		}

		public Vector2 GetPointOnSpline(CurveData c, float step) {
			var point = Utility.CurveUtil.GetPointCubic(c.Start, c.TangentOne, c.TangentTwo, c.End, step);
			point = Utility.ConvertPointToWorldSpace(transform, point);

			return point;
		}

		public Vector2 GetPointOnSpline(Vector2 start, Vector2 end, Vector2 t1, Vector2 t2, float normalizedLocation) {
			var point = Utility.CurveUtil.GetPointCubic(start, t1, t2, end, normalizedLocation);
			point = Utility.ConvertPointToWorldSpace(transform, point);

			return point;
		}

		public override void SetPoint(int index, Vector2 point) {
			if (index % 3 == 0) {
				var delta = point - _points[index];

				if (Loop)
					OffsetPointsWithLoop(index, point, delta);

				else
					OffsetPoints(index, delta);
			}

			_points[index] = point;
			EnforceConnection(index);
		}

		void EnforceConnection(int index) {
			var connectionIndex = (index + 1) / 3;
			var connectionType  = _connections[connectionIndex];

			if (connectionType == Utility.Spline.Connection.Free || (!Loop &&
			                                                         (connectionIndex == 0 ||
			                                                          connectionIndex == _connections.Length - 1)))
				return;

			int enforcedIndex;
			var middleIndex = connectionIndex * 3;

			var fixedIndex = index <= middleIndex
				                 ? EnforcePointsLeftOfMiddle(middleIndex, out enforcedIndex)
				                 : EnforcePointsRightOfMiddle(middleIndex, out enforcedIndex);

			var middlePoint     = _points[middleIndex];
			var enforcedTangent = middlePoint - _points[fixedIndex];

			if (connectionType == Utility.Spline.Connection.Aligned)
				enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middlePoint, _points[enforcedIndex]);

			_points[enforcedIndex] = enforcedTangent + middlePoint;
		}

		int EnforcePointsRightOfMiddle(int middleIndex, out int enforcedIndex) {
			var fixedIndex = middleIndex + 1;

			if (fixedIndex >= _points.Length)
				fixedIndex = 1;

			enforcedIndex = middleIndex - 1;

			if (enforcedIndex < 0)
				enforcedIndex = _points.Length - 2;

			return fixedIndex;
		}

		int EnforcePointsLeftOfMiddle(int middleIndex, out int enforcedIndex) {
			var fixedIndex = middleIndex - 1;

			if (fixedIndex < 0)
				fixedIndex = _points.Length - 2;

			enforcedIndex = middleIndex + 1;

			if (enforcedIndex >= _points.Length)
				enforcedIndex = 1;

			return fixedIndex;
		}

		void OffsetPointsWithLoop(int index, Vector2 point, Vector2 delta) {
			if (index == 0) {
				_points[1]                  += delta;
				_points[_points.Length - 2] += delta;
				_points[_points.Length - 1] =  point;
			}

			else if (index == _points.Length - 1) {
				_points[0]         =  point;
				_points[1]         += delta;
				_points[index - 1] += delta;
			}

			else {
				_points[index - 1] += delta;
				_points[index + 1] += delta;
			}
		}

		void OffsetPoints(int index, Vector2 delta) {
			if (index > 0)
				_points[index - 1] += delta;

			if (index + 1 < _points.Length)
				_points[index + 1] += delta;
		}

		public Utility.Spline.Connection GetConnection(int index) {
			if (index < 0 || index > ConnectionCount)
				return Utility.Spline.Connection.Free;

			return _connections[(index + 1) / 3];
		}

		public void SetConnection(int index, Utility.Spline.Connection connection) {
			var connectionIndex = (index + 1) / 3;
			_connections[connectionIndex] = connection;

			if (Loop) {
				if (connectionIndex == 0)
					_connections[_connections.Length - 1] = connection;
				else if (connectionIndex == _connections.Length - 1)
					_connections[0] = connection;
			}

			EnforceConnection(index);
		}

		void ValidateConnectionArraySize() {
			if (_connections.Length != _currentConnectionCount)
				Array.Resize(ref _connections, _currentConnectionCount);

			if (_connections.Length < 2)
				Array.Resize(ref _connections, 2);
		}

		void ValidatePointArraySize() {
			if (_points.Length != _currentPointCount)
				Array.Resize(ref _points, _currentPointCount);
		}

		void ValidateLoopToggle() {
			if (_loopSpline == Toggle.No)
				return;

			_connections[^1] = _connections[0];
			SetPoint(0, _points[0]);
		}

#region Plumbing

		[VerticalGroup("Settings/Vertical", AnimateVisibility = true, PaddingBottom = 5f, PaddingTop = 5f)]
		[LabelText("Loop Spline")]
		[LabelWidth(200)]
		[PropertyOrder(10)]
		[PropertySpace(10, 10)]
		[Indent]
		[EnumToggleButtons]
		[OnValueChanged("ValidateLoopToggle")]
		[SerializeField]
		Toggle _loopSpline = Toggle.No;

		[TitleGroup("Polynomial Shape", "Control of lines, curves, & splines", Indent = true)]
		[LabelWidth(200)]
		[Indent]
		[PropertySpace(10, 0)]
		[GUIColor(80 / 255f, 210 / 255f, 180 / 255f)]
		[Button("Add Curve", ButtonSizes.Medium, ButtonStyle.CompactBox)]
		[HorizontalGroup("Polynomial Shape/Buttons")]
		void AddCurve() {
			var endPoint = LastPoint;

			Array.Resize(ref _points,      _points.Length      + 3);
			Array.Resize(ref _connections, _connections.Length + 1);

			endPoint.x  += 1;
			_points[^3] =  endPoint;
			endPoint.x  += 1;
			_points[^2] =  endPoint;
			endPoint.x  += 1;
			_points[^1] =  endPoint;

			_connections[^1] = _connections[^2];

			EnforceConnection(_points.Length - 4);

			if (Loop) {
				_points[^1]      = _points[0];
				_connections[^1] = _connections[0];
				EnforceConnection(0);
			}

			_currentPointCount      = _points.Length;
			_currentConnectionCount = _connections.Length;
		}

		[TitleGroup("Polynomial Shape", "Control of lines, curves, & splines", Indent = true)]
		[LabelWidth(150)]
		[Indent]
		[PropertySpace(10, 0)]
		[GUIColor(141 / 255f, 61 / 255f, 175 / 255f)]
		[HorizontalGroup("Polynomial Shape/Buttons")]
		[Button("Remove Curve", ButtonSizes.Medium, ButtonStyle.CompactBox)]
		void RemoveCurve() {
			if (_points.Length <= 4) {
				Utility.Log.Warning("You are trying to remove the root curve of the spline, which cannot be done.");
				return;
			}

			Array.Resize(ref _points,      _points.Length      - 3);
			Array.Resize(ref _connections, _connections.Length - 1);

			_currentPointCount      = _points.Length;
			_currentConnectionCount = _connections.Length;
		}

		[TitleGroup("Polynomial Shape", "Control of lines, curves, & splines", Indent = true)]
		[LabelWidth(150)]
		[Indent]
		[PropertySpace(10, 20)]
		[HorizontalGroup("Polynomial Shape/Buttons")]
		[GUIColor(254 / 255f, 214 / 255f, 82 / 255f)]
		[Button("Reset Control Points", ButtonSizes.Medium, ButtonStyle.CompactBox)]
		protected override void Reset() {
			_points      = new[] { Vector2.zero, 2 * Vector2.one, 3 * Vector2.one, 4 * Vector2.one };
			_connections = Utility.Defaults.DefaultConnections;

			_currentPointCount      = 4;
			_currentConnectionCount = 2;
		}

		[TitleGroup("Polynomial Shape", "Control of lines, curves, & splines", Indent = true)]
		[LabelText("Curve Connections", true)]
		[Tooltip("Free: Allows for free movement of all point nodes\n" +
		         "Aligned: TBD\n"                                      +
		         "Mirrored: TBD")]
		[PropertyOrder(-11)]
		[LabelWidth(150)]
		[Indent]
		[PropertySpace(10, 0)]
		[ListDrawerSettings(Expanded = true, DraggableItems = true, ShowIndexLabels = true, NumberOfItemsPerPage = 4)]
		[SerializeField]
		[EnumToggleButtons]
		Utility.Spline.Connection[] _connections = Utility.Defaults.DefaultConnections;

		[TitleGroup("Point Extraction", "Control of lines, curves, & splines", Indent = true)]
		[HorizontalGroup("Point Extraction/SplineDisplays")]
		[LabelWidth(200)]
		[Indent]
		[PropertySpace(10, 0)]
		[GUIColor(84 / 255f, 195 / 255f, 255 / 255f)]
		[Button("Extract", ButtonSizes.Medium, ButtonStyle.CompactBox)]
		protected override void ExtractPoints() {
			var extractor = new SplinePointExtractor();
			extractor.EnableLogging();
			var data = extractor.Extract(this, _steps);

			_extractedPoints = data.Points;
		}

#endregion
	}
}