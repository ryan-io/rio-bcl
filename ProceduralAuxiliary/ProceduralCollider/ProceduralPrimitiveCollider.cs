using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// by Pepijn Willekens
// https://twitter.com/PepijnWillekens
// pepijnwillekens@gmail.com
namespace ProceduralAuxiliary.ProceduralCollider {
#if UNITY_2018_3_OR_NEWER
    [ExecuteAlways]
#else
	[ExecuteInEditMode]
#endif
	public class ProceduralPrimitiveCollider : MonoBehaviour {
		[HideInInspector] public Transform colliderContainer;

		[HideInInspector] public List<Transform> corners = new();

		[Header("Settings")] public float radius = 0.5f;

		public float          heigth = 2;
		public float          depth;
		public float          extraWidth;
		public bool           invert;
		public bool           loop;
		public bool           makeRenderers;
		public PhysicMaterial physicsMaterial;

		[Header("Gizmos")] public bool onlyWhenSelected;

		public float GizmoLineInterval = 0.5f;

		public Color GizmoColor = Color.green;

		[Tooltip("Enables disables hiding of the collider objects")]
		public bool DEBUG;

		[HideInInspector] public int lastHash;

		readonly List<Vector2> cachedCornerList         = new();
		readonly List<Vector2> cachedCornerListToOutset = new();

#if UNITY_EDITOR || UNITY_STANDALONE
		Mesh     _cubeMesh;
		Mesh     _cylinderMesh;
		Material _defaultMaterial;

		Mesh CubeMesh {
			get {
				if (_cubeMesh == null) FixPrimitiveRefs();

				return _cubeMesh;
			}
		}

		Mesh CylinderMesh {
			get {
				if (_cylinderMesh == null) FixPrimitiveRefs();

				return _cylinderMesh;
			}
		}

		Material DefaultMaterial {
			get {
				if (_defaultMaterial == null) FixPrimitiveRefs();

				return _defaultMaterial;
			}
		}

		void FixPrimitiveRefs() {
			var cube     = GameObject.CreatePrimitive(PrimitiveType.Cube);
			var cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			_cubeMesh        = cube.GetComponent<MeshFilter>().sharedMesh;
			_cylinderMesh    = cylinder.GetComponent<MeshFilter>().sharedMesh;
			_defaultMaterial = cube.GetComponent<MeshRenderer>().sharedMaterial;
			if (Application.isPlaying) {
				Destroy(cube);
				Destroy(cylinder);
			}
			else {
				DestroyImmediate(cube);
				DestroyImmediate(cylinder);
			}
		}

		int CalculateSettingsHash() {
			unchecked {
				var hashCode = 0;
				for (var i = 0; i < corners.Count; i++)
					if (corners[i] != null)
						hashCode = (hashCode * 397) ^ corners[i].localPosition.GetHashCode();

				hashCode = (hashCode * 397) ^ radius.GetHashCode();
				hashCode = (hashCode * 397) ^ makeRenderers.GetHashCode();
				hashCode = (hashCode * 397) ^ transform.childCount.GetHashCode();
				hashCode = (hashCode * 397) ^ heigth.GetHashCode();
				hashCode = (hashCode * 397) ^ depth.GetHashCode();
				hashCode = (hashCode * 397) ^ extraWidth.GetHashCode();
				hashCode = (hashCode * 397) ^ invert.GetHashCode();
				hashCode = (hashCode * 397) ^ loop.GetHashCode();
				hashCode = (hashCode * 397) ^ gameObject.layer.GetHashCode();
				hashCode = (hashCode * 397) ^ gameObject.isStatic.GetHashCode();
				hashCode = (hashCode * 397) ^ (physicsMaterial ? physicsMaterial.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ DEBUG.GetHashCode();
				return hashCode;
			}
		}

		public int loopInt() => loop ? 0 : -1;

		void Update() {
			if (Application.isPlaying) return;

			var lossyScale = transform.lossyScale;
			transform.localScale = Vector3.Scale(transform.localScale,
				new Vector3(1 / lossyScale.x, 1 / lossyScale.y, 1 / lossyScale.z));


			var newHash = CalculateSettingsHash();
			if (newHash != lastHash) {
				lastHash = newHash;

				corners.Clear();
				for (var i = 0; i < transform.childCount; i++) {
					var child = transform.GetChild(i);
					if (child != colliderContainer) {
						corners.Add(child);
						if (CanBeEdited()) child.gameObject.name = corners.Count.ToString();
					}
				}

				if (CanBeEdited()) MakeColliders();
			}

			for (var i = 0; i < transform.childCount; i++) {
				var child = transform.GetChild(i);
				if (child != colliderContainer)
					child.gameObject.hideFlags = DEBUG || CanBeEdited() ? HideFlags.None : HideFlags.HideInHierarchy;
			}

			ColliderContainer.hideFlags = DEBUG ? HideFlags.None : HideFlags.HideInHierarchy;
		}

		public Transform ColliderContainer {
			get {
				if (colliderContainer == null) {
					colliderContainer = new GameObject("Colliders").transform;
					colliderContainer.SetParent(transform);
					colliderContainer.Reset();
				}

				return colliderContainer;
			}
		}

		public void MakeColliders() {
			ColliderContainer.DestroyAllChildren();
			for (var i = 0; i < corners.Count; i++) corners[i].localPosition = corners[i].localPosition.ChangeY(0);

			if (corners.Count >= 3)
				MakeAllColliders();
			else
				ForcePopulateCorners();
		}

#if UNITY_EDITOR|| UNITY_STANDALONE
		public void ForcePopulateCorners() {
			for (var i = 0; i < 3; i++) {
				var newObj = new GameObject().transform;
				newObj.SetParent(transform);
				newObj.localPosition   = GizmoLineInterval * 5 * Vector3.forward * i;
				newObj.gameObject.name = i.ToString();
				PolygonColliderEditorExtention.DrawIcon(newObj.gameObject, 0);
				corners.Add(newObj);
			}
		}
#endif

		public void MakeAllColliders() {
			MakeAllEdgeColliders(InSetCorners(FillCachedCornerList(corners), radius));
			if (radius > 0) MakeAllCornerColliders(InSetCorners(FillCachedCornerList(corners), radius));
		}

		public bool CanBeEdited() {
#if UNITY_2018_3_OR_NEWER
            return PrefabUtility.GetPrefabInstanceStatus(gameObject) == PrefabInstanceStatus.NotAPrefab;
#else
			return true;
#endif
		}

		public void MakeAllEdgeColliders(List<Vector2> corners) {
			for (var i = 0; i < corners.Count + loopInt(); i++)
				MakeEdgeCollider(string.Format("{0} - {1}", i + 1, (i + 1) % corners.Count + 1), corners[i],
					corners[(i + 1) % corners.Count], radius, heigth + depth, depth, extraWidth);
		}

		public void MakeAllCornerColliders(List<Vector2> corners) {
			for (var i = 0; i < corners.Count; i++)
				MakeCornerCollider(string.Format("{0}", i + 1), corners[i], radius, heigth + depth, depth);
		}

		void MakeEdgeCollider(string name, Vector2 from, Vector2 to, float radius, float heigth, float yOffset,
			float extraWidth) {
			var go = new GameObject("EdgeCollider " + name);
			go.layer    = gameObject.layer;
			go.isStatic = gameObject.isStatic;
			go.transform.SetParent(ColliderContainer);
			go.AddComponent<BoxCollider>().sharedMaterial = physicsMaterial;
			if (makeRenderers) {
				go.AddComponent<MeshFilter>().sharedMesh       = CubeMesh;
				go.AddComponent<MeshRenderer>().sharedMaterial = DefaultMaterial;
			}

			go.transform.localPosition = To3D((from + to) / 2 + Perp(from, to, true) * extraWidth / 2)
.ChangeY(heigth                                           / 2 - yOffset);
			go.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(To3D((to - from).normalized)),
				transform.TransformDirection(Vector3.up));
			go.transform.localScale = new Vector3(radius * 2 + extraWidth, heigth, Vector2.Distance(from, to));
		}

		void MakeCornerCollider(string name, Vector2 current, float radius, float heigth, float yOffset) {
			var go = new GameObject("CornerCollider " + name);
			go.layer    = gameObject.layer;
			go.isStatic = gameObject.isStatic;
			go.transform.SetParent(ColliderContainer);
			var collider = go.AddComponent<CapsuleCollider>();
			go.transform.localPosition = To3D(current).ChangeY(heigth / 2 - yOffset);
			collider.radius            = radius;
			collider.height            = heigth + radius * 2;
			collider.sharedMaterial    = physicsMaterial;
			go.transform.localRotation = Quaternion.identity;

			if (makeRenderers) {
				var r = new GameObject("CornerCollider");
				r.layer    = gameObject.layer;
				r.isStatic = gameObject.isStatic;
				r.transform.SetParent(ColliderContainer);
				r.AddComponent<MeshFilter>().sharedMesh       = CylinderMesh;
				r.AddComponent<MeshRenderer>().sharedMaterial = DefaultMaterial;
				r.transform.localPosition                     = To3D(current).ChangeY(heigth / 2 - yOffset);
				r.transform.localScale                        = new Vector3(radius * 2, heigth / 2, radius * 2);
				r.transform.localRotation                     = Quaternion.identity;
			}
		}

		List<Vector2> FillCachedCornerList(List<Transform> corners) {
			cachedCornerList.Clear();
			cachedCornerList.AddRange(corners.Select(e => To2D(e.localPosition)));
			if (invert) cachedCornerList.Reverse();

			return cachedCornerList;
		}

		List<Vector2> InSetCorners(List<Vector2> corners, float radius) {
			cachedCornerListToOutset.Clear();
			for (var i = 0; i < corners.Count; i++) {
				var prev                  = corners[(i - 1 + corners.Count) % corners.Count];
				var cur                   = corners[i];
				var next                  = corners[(i + 1) % corners.Count];
				if (!loop && i == 0) prev = cur + (cur - next);

				if (!loop && i == corners.Count - 1) next = cur + (cur - prev);

				cachedCornerListToOutset.Add(InsetCorner(prev, cur, next, radius));
			}

			corners.Clear();
			corners.AddRange(cachedCornerListToOutset);

			return corners;
		}

		Vector2 InsetCorner(Vector2 prev, Vector2 current, Vector2 next, float radius) {
			var nextdir = (next - current).normalized;
			var prevdir = (prev - current).normalized;
			var perpdir = Perp(current, prev, false);

			var cos = Mathf.Cos(Vector2.Angle(perpdir, nextdir) * Mathf.Deg2Rad);
			var d   = radius / cos;

			if (Mathf.Abs(cos) < 0.00001f) return current + perpdir * radius;

			return current
			       + d * nextdir
			       + d * prevdir;
		}

		void OnDrawGizmos() {
			if (!onlyWhenSelected && isActiveAndEnabled) DrawGizmos();
		}

		void OnDrawGizmosSelected() {
			if (onlyWhenSelected && isActiveAndEnabled) DrawGizmos();
		}

		void DrawGizmos() {
#if UNITY_EDITOR
			
			var prevHandlesColor         = Handles.color;
			var prevGizmosColor          = Gizmos.color;
			Gizmos.color = Handles.color = GizmoColor;

			if (corners.Count >= 3) DrawPolygon(InSetCorners(FillCachedCornerList(corners), radius));

			Handles.color = prevHandlesColor;
			Gizmos.color  = prevGizmosColor;
#endif
		}

		void DrawPolygon(List<Vector2> corners) {
			for (var i = 0; i < corners.Count + loopInt(); i++) {
				DrawEdge(corners[i], corners[(i + 1) % corners.Count], radius);
				if (i < corners.Count + loopInt() * 2)
					DrawCorner(corners[i], corners[(i + 1) % corners.Count], corners[(i + 2) % corners.Count], radius);
			}
		}

		void DrawEdge(Vector2 from, Vector2 to, float radius) {
			var offset = Perp(from, to, false) * radius;
			var start  = To3D(from + offset);
			var end    = To3D(to   + offset);
			DrawLine(start,                 end);
			DrawLine(start.ChangeY(-depth), end.ChangeY(-depth));
			DrawLine(start.ChangeY(heigth), end.ChangeY(heigth));


			var distance = Vector3.Distance(start, end);
			var n        = Mathf.CeilToInt(distance / GizmoLineInterval);
			n = Mathf.Min(n, 250);
			for (var i = 0; i <= n; i++)
				if (n != 0)
					DrawBar(Vector3.Lerp(start, end, (float)i / n));
		}

		void DrawCorner(Vector2 prev, Vector2 current, Vector2 next, float radius) {
			DrawCornerLine(prev, current, next, radius, -depth);
			DrawCornerLine(prev, current, next, radius, 0);
			DrawCornerLine(prev, current, next, radius, heigth);

			var angle    = FromToAngle(prev, current, next);
			var distance = angle / 360 * radius * 2 * Mathf.PI;

			var n        = Mathf.CeilToInt(distance / GizmoLineInterval);
			var startPos = Perp(prev, current, false) * this.radius;

			n = Mathf.Min(n, 250);
			for (var i = 0; i <= n; i++) {
				var lerpVal = (float)i / n;
				if (!(float.IsNaN(angle) || n == 0))
					DrawBar(To3D(current) + Quaternion.Euler(0, Mathf.Lerp(0, angle, lerpVal), 0) * To3D(startPos));
			}
		}

		void DrawBar(Vector3 pos) {
			DrawLine(pos.ChangeY(-depth), pos.ChangeY(heigth));
		}

		void DrawBar(Vector2 pos) {
			DrawBar(To3D(pos));
		}

		void DrawCornerLine(Vector2 prev, Vector2 current, Vector2 next, float radius, float heigth) {
#if UNITY_EDITOR
			
			Handles.DrawWireArc(
				transform.TransformPoint(To3D(current).ChangeY(heigth)),
				transform.TransformDirection(Vector3.up),
				transform.TransformDirection(To3D(Perp(prev, current, false))),
				FromToAngle(prev, current, next),
				radius
			);
#endif
		}

		void DrawLine(Vector3 from, Vector3 to) {
			Gizmos.DrawLine(transform.TransformPoint(from), transform.TransformPoint(to));
		}


		Vector3 To3D(Vector2 vec) => new(vec.x, 0, vec.y);

		Vector2 To2D(Vector3 vec) => new(vec.x, vec.z);

		Vector2 Perp(Vector2 dir, bool inverse) {
			if (inverse)
				return new Vector2(dir.y, -dir.x);
			return new Vector2(-dir.y, dir.x);
		}

		Vector2 Perp(Vector2 from, Vector2 to, bool inverse) {
			var dir = to - from;
			return Perp(dir, inverse).normalized;
		}

		float Atan2angle(Vector2 vector2) => Mathf.Atan2(vector2.y, vector2.x) * Mathf.Rad2Deg;

		float FromToAngle(Vector2 prev, Vector2 current, Vector2 next)
			=> (Atan2angle(prev - current) - Atan2angle(current - next) + 360) % 360;

		void OnDestroy() {
			if (colliderContainer) {
				if (Application.isPlaying)
					Destroy(colliderContainer.gameObject);
				else
					DestroyImmediate(colliderContainer.gameObject);
			}
		}
#endif
	}
}