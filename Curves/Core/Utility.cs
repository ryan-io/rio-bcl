using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Curves {
	public static class Utility {
		public static Vector2 ConvertPointToWorldSpace(Transform transform, Vector2 point)
			=> transform.TransformPoint(point);

		public static Vector2 ConverToLocalSpace(Transform transform, Vector2 pointToCheck)
			=> transform.InverseTransformPoint(pointToCheck);

		public static class Strings {
			public const string Slash        = "/";
			public const string SaveToFolder = "Splines - Extracted Points Data";
			public const string AssetSuffix  = ".asset";
			public const string AssetsFolder = "Assets/";
			public const string ParentFolder = "Lines, Curve, and Splines";
		}

		public static class Operations {
#if UNITY_EDITOR || UNITY_STANDALONE

			public static void Record(Object obj, string label = "Move") {
				Undo.RecordObject(obj, label);
				EditorUtility.SetDirty(obj);
			}

			public static void FitViewToScreen(Transform tr, IReadOnlyList<Vector2> points) {
				Debug.Log("Calling FitView");
				var sceneCamera = EditorWindow.GetWindow<SceneView>().camera;
				var minX        = points.Min(point => point.x);
				var maxX        = points.Max(point => point.x);
				var miny        = points.Min(point => point.y);
				var maxY        = points.Max(point => point.y);

				var vectorMin = new Vector2(minX, miny);
				vectorMin = ConvertPointToWorldSpace(tr, vectorMin);

				var vectorMax = new Vector2(maxX, maxY);
				vectorMax = ConvertPointToWorldSpace(tr, vectorMax);

				sceneCamera.rect = Rect.MinMaxRect(vectorMin.x, vectorMin.y, vectorMax.x, vectorMax.y);
			}

#endif
		}

		public static class Defaults {
			public static readonly Spline.Connection[] DefaultConnections = {
				Spline.Connection.Free, Spline.Connection.Free
			};

			static readonly Vector2[] LinePoints = { Vector2.zero, 50 * Vector2.one };

			static readonly Vector2[] CurvePointsQuadratic = { Vector2.zero, Vector2.one, 50 * Vector2.one };

			static readonly Vector2[] SplinePoints =
				{ Vector2.zero, 25 * Vector2.one, 50 * Vector2.one, 100 * Vector2.one };

			public static Vector2[] GetDefaultPoints(SplineType type) {
				switch (type) {
					case SplineType.Line:
						return LinePoints;
					case SplineType.QuadraticCurve:
						return CurvePointsQuadratic;
					case SplineType.CubicCurve:
					case SplineType.Spline:
						return SplinePoints;
					default:
						return LinePoints;
				}
			}
		}

		public static class Log {
			const string NoPoints = "Cannot save. Extracted points array does not contain any members.";

			public static void Message(string message)
				=> Debug.Log(message);

			public static void Warning(string message)
				=> Debug.LogWarning(message);

			public static void Error(string message)
				=> Debug.LogError(message);

			public static void LogNoPointsWarning()
				=> Debug.LogWarning(NoPoints);
		}

		public static class Constants {
			public const float HandleSize = 0.06f;

			public const float PickSize = 0.085f;

			public const float LateralConstant = 0.05f;

			public const float VerticalConstant = 0.4f;

			public const float VerticalConstantStartAndEnd = 0.08f;
		}

		public static class Handle {
#if UNITY_EDITOR || UNITY_STANDALONE

			public static float GetSize(Vector2 point)
				=> HandleUtility.GetHandleSize(point);

			public static bool IsSelected(Vector2 point, Quaternion rotation, float sizeScalar)
				=> Handles.Button(
					point, rotation, sizeScalar * Constants.HandleSize,
					sizeScalar                  * Constants.PickSize, Handles.DotHandleCap);

#endif
		}

		public static class Colors {
			public static Color SalmonPink = ConvertFromRGBA(242, 140, 164);

			public static Color GetConnectionColor(Spline.Connection connection) {
				switch (connection) {
					case Spline.Connection.Free:
						return Color.white;
					case Spline.Connection.Aligned:
						return Color.cyan;
					case Spline.Connection.Mirrored:
						return Color.magenta;
					default:
						return Color.white;
				}
			}

			public static Color ConvertFromRGBA(float red, float green, float blue, float alpha = 1.0f)
				=> new Color(red / 255f, green / 255f, blue / 255f, alpha);
		}

		public static class LineUtil {
			public static Vector2 GetInterpolatedPoint(Transform parent, Vector2 start, Vector2 end, float step) {
				var point = Vector2.Lerp(start, end, step);
				point = ConvertPointToWorldSpace(parent, point);

				return point;
			}

			public static float GetLength(Line2D line)
				=> (line.End - line.Start).magnitude;

			public static float GetDistance(Vector2 start, Vector2 end)
				=> Vector2.Distance(start, end);

			public static bool IsWithinError(Vector2 start, Vector2 end, float error)
				=> GetDistance(start, end) <= error;
		}

		public static class CurveUtil {
			public static Vector2 GetPointQuadratic(Vector2 start, Vector2 midPoint, Vector2 end, float step) {
				step = Mathf.Clamp01(step);
				var offset = 1f - step;

				var value = start * (offset * offset)        +
				            2f    * offset * step * midPoint +
				            step  * step   * end;

				return value;
			}

			public static Vector2 GetPointCubic(Vector2 start, Vector2 tangent1, Vector2 tangent2, Vector2 end,
				float normalizedLocation) {
				normalizedLocation = Mathf.Clamp01(normalizedLocation);
				var offset = 1f - normalizedLocation;

				var value = offset             * offset * offset * start +
				            3f                 * offset * offset * normalizedLocation * tangent1 +
				            3f                 * offset * normalizedLocation * normalizedLocation * tangent2 +
				            normalizedLocation * normalizedLocation * normalizedLocation * end;

				return value;
			}

			public static Vector2
				GetFirstDerivativeQuadratic(Vector2 start, Vector2 midPoint, Vector2 end, float step) {
				var value = 2 * (1 - step) * (midPoint - start) + 2 * step * (end - midPoint);

				return value;
			}

			public static Vector2 GetFirstDerivativeCubic(Vector2 start, Vector2 tangent1, Vector2 tangent2,
				Vector2 end, float step) {
				step = Mathf.Clamp01(step);
				var offset = 1f - step;

				var value = 3f * offset * offset * (tangent1 - start)    +
				            6f * offset * step   * (tangent2 - tangent1) +
				            3f * step   * step   * (end      - tangent2);

				return value;
			}

			public static void DrawCurveQuadratic(Curve2D polynomial) {
#if UNITY_EDITOR

				var start = polynomial.GetPointQuadratic(0f);
				start = ConvertPointToWorldSpace(polynomial.transform, start);

				for (var i = 1; i <= polynomial._steps; i++) {
					var end = polynomial.GetPointQuadratic(i / (float)polynomial._steps);
					end = ConvertPointToWorldSpace(polynomial.transform, end);
					Handles.DrawLine(start, end);
					start = end;
				}

#endif
			}

			public static void DrawCurveQuadratic(IReadOnlyList<Vector2> points, Transform handleTr) {
#if UNITY_EDITOR

				var start = points[0];
				var count = points.Count;
				start = ConvertPointToWorldSpace(handleTr.transform, start);

				for (var i = 1; i < count; i++) {
					var end = ConvertPointToWorldSpace(handleTr.transform, points[i]);
					Handles.DrawLine(start, end);
					start = end;
				}

#endif
			}


			public static void DrawCurveCubic(Curve2D polynomial) {
				var p1 = polynomial.GetPoint(0);
				var p2 = polynomial.GetPoint(3);
				var p3 = polynomial.GetPoint(1);
				var p4 = polynomial.GetPoint(2);

				p1 = ConvertPointToWorldSpace(polynomial.transform, p1);
				p2 = ConvertPointToWorldSpace(polynomial.transform, p2);
				p3 = ConvertPointToWorldSpace(polynomial.transform, p3);
				p4 = ConvertPointToWorldSpace(polynomial.transform, p4);

				Handles.DrawBezier(
					p1, p2, p3, p4,
					polynomial.SerializedColor, null, 2f);
			}
		}

		public static class Spline {
			[Serializable]
			public enum Connection {
				Free,
				Aligned,
				Mirrored
			}

			public static int ProcessStep(Spline2D spline, float step) {
				int i;

				if (step >= 1) {
					step = 1;
					i    = spline.PointCount - 4;
				}

				else {
					step =  Mathf.Clamp01(step) * spline.CurveCount;
					i    =  (int)step;
					step -= i;
					i    *= 3;
				}

				return i;
			}
		}
	}
}