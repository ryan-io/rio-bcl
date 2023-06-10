#if UNITY_EDITOR || UNITY_STANDALONE

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UnityBCL {
	public class PolygonColliderGenerator : MonoBehaviour {
		[EnumToggleButtons] [SerializeField] Type       _generationQuality = Type.High;
		[SerializeField]                     GameObject _gameObject        = null!;

		[Button]
		void Generate() {
			if (!_gameObject || Application.isPlaying) return;

			RemoveColliders();

			var col    = _gameObject.AddComponent<PolygonCollider2D>();
			var sprite = _gameObject.GetComponent<SpriteRenderer>().sprite;

			switch (_generationQuality) {
				case Type.High:
					GenerateHighQuality(col, sprite);
					break;
				case Type.Low:
					var mesh = HandleSpriteRenderer();
					GenerateLowQuality(mesh, col);
					break;
				default:
					return;
			}
		}

		static void GenerateHighQuality(PolygonCollider2D col, Sprite sprite) {
			var points           = new List<Vector2>();
			var simplifiedPoints = new List<Vector2>();

			col.pathCount = sprite.GetPhysicsShapeCount();

			for (var i = 0; i < col.pathCount; i++) {
				sprite.GetPhysicsShape(i, points);
				LineUtility.Simplify(points, 0.05f, simplifiedPoints);
				col.SetPath(i, simplifiedPoints);
			}
		}

		static void GenerateLowQuality(Mesh? mesh, PolygonCollider2D col) {
			var edges = new Dictionary<string, KeyValuePair<int, int>>();
			if (mesh != null) {
				var triangles = mesh.triangles;
				var vertices  = mesh.vertices;

				for (var i = 0; i < triangles.Length; i += 3) {
					for (var e = 0; e < 3; e++)
						ExtractEdges(triangles, i, e, edges);
				}

				var lookup = new Dictionary<int, int>();

				foreach (var edge in edges.Values)
					LookupEdge(lookup, edge);

				col.pathCount = 0;

				var startVert    = 0;
				var nextVert     = startVert;
				var highestVert  = startVert;
				var colliderPath = new List<Vector2>();

				while (true) {
					if (nextVert >= vertices.Length)
						break;

					colliderPath.Add(vertices[nextVert]);
					nextVert = lookup[nextVert];

					if (nextVert > highestVert)
						highestVert = nextVert;

					if (nextVert == startVert) {
						var pathCount = col.pathCount;
						pathCount++;
						col.pathCount = pathCount;
						col.SetPath(pathCount - 1, colliderPath.ToArray());
						colliderPath.Clear();

						if (lookup.ContainsKey(highestVert + 1)) {
							startVert = highestVert + 1;
							nextVert  = startVert;
							continue;
						}

						break;
					}
				}
			}
		}

		Mesh? HandleSpriteRenderer() {
			var r = _gameObject.GetComponentRecursive<SpriteRenderer>(out var parent);

			if (r == null) return null;

			var mesh   = new Mesh();
			var sprite = r.sprite;

			mesh.vertices  = Array.ConvertAll(sprite.vertices, i => (Vector3)i);
			mesh.uv        = sprite.uv;
			mesh.triangles = Array.ConvertAll(sprite.triangles, i => (int)i);

			return mesh;
		}

		void RemoveColliders() {
			var colliders = _gameObject.GetComponents<Collider2D>();

			if (colliders.Length > 0) {
				var count = colliders.Length;

				for (var i = 0; i < count; i++)
					DestroyImmediate(colliders[i]);
			}
		}

		static void LookupEdge(IDictionary<int, int> lookup, KeyValuePair<int, int> edge) {
			if (lookup.ContainsKey(edge.Key) == false)
				lookup.Add(edge.Key, edge.Value);
		}

		static void ExtractEdges(IReadOnlyList<int> triangles, int i, int e,
			IDictionary<string, KeyValuePair<int, int>> edges) {
			var vert1 = triangles[i + e];
			var vert2 = triangles[i + e + 1 > i + 2 ? i : i + e + 1];
			var edge  = Mathf.Min(vert1, vert2) + ":" + Mathf.Max(vert1, vert2);

			if (edges.ContainsKey(edge))
				edges.Remove(edge);
			else
				edges.Add(edge, new KeyValuePair<int, int>(vert1, vert2));
		}

		enum Type {
			High,
			Low
		}
	}
}

#endif