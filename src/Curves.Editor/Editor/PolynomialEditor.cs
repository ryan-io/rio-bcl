#if UNITY_EDITOR || UNITY_STANDALONE

using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Curves.Editor {
	public abstract class PolynomialEditor : OdinEditor {
		protected Quaternion HandleRotation;

		protected          Transform HandleTransform = null!;
		protected abstract void      OnSceneGUI();

		protected static void SetColor(IColor color)
			=> Handles.color = color.SerializedColor;

		protected static void DrawLine(Vector2 pointStart, Vector2 pointEnd)
			=> Handles.DrawDottedLine(pointStart, pointEnd, 2.5f);

		protected static Quaternion GetRotation(Transform transform)
			=> Tools.pivotRotation == PivotRotation.Local ? transform.rotation : Quaternion.identity;

		protected static Vector2 CreatePositionHandles(Vector2 position, Quaternion rotation)
			=> Handles.DoPositionHandle(position, rotation);

		protected void LabelPoints(Transform parent,
			IReadOnlyList<Vector2> points, float radius, string tangentLabel, bool shouldLoop = false) {
			var startPoint = Utility.ConvertPointToWorldSpace(parent, points[0]);
			ManualDrawLabel(startPoint, "Start");

			if (!shouldLoop) {
				var endPoint = Utility.ConvertPointToWorldSpace(parent, points[points.Count - 1]);
				ManualDrawLabel(endPoint, "End");
			}

			for (var i = 1; i < points.Count - 1; i++)
				DrawTangentDisplay(parent, points, radius, tangentLabel, i);
		}

		static void DrawTangentDisplay(
			Transform parent, IReadOnlyList<Vector2> points, float radius, string tangentLabel, int i) {
			var position = Utility.ConvertPointToWorldSpace(parent, points[i]);
			var scalar   = Utility.Handle.GetSize(position);

			Handles.DrawWireDisc(position, new Vector3(0, 0, 1), scalar * radius);
			position.x -= scalar * Utility.Constants.LateralConstant;
			position.y -= scalar * Utility.Constants.VerticalConstant;
			Handles.Label(position, $"{tangentLabel} " + i);
		}

		static void ManualDrawLabel(Vector2 pointPosition, string label) {
			var handleSize = Utility.Handle.GetSize(pointPosition);

			pointPosition.x -= handleSize * Utility.Constants.LateralConstant;
			pointPosition.y -= handleSize * Utility.Constants.VerticalConstantStartAndEnd;

			Handles.Label(pointPosition, label);
		}

		protected void ValidateDrawOptionals(Polynomial? polynomial) {
			if (polynomial!.ShouldDrawRootHandles)
				AddParentHandles();
			else
				RemoveParentHandles();
		}

		void RemoveParentHandles() => Tools.current = Tool.None;

		void AddParentHandles() => Tools.current = Tool.Move;
	}
}
#endif