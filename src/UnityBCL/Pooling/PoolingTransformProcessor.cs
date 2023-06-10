using UnityEngine;

namespace UnityBCL {
	public static class PoolingTransformProcessor {
		public static void SetPositionAndRotation(PoolJob.SpawnObjectJobQuat job, GameObject objectToSpawn) {
			objectToSpawn.transform.position = job.Position;
			objectToSpawn.transform.rotation = job.Rotation;
		}

		public static void SetPositionAndRotation(PoolJob.SpawnObjectJobEuler job, GameObject objectToSpawn) {
			objectToSpawn.transform.position = job.Position;
			objectToSpawn.transform.rotation = Quaternion.Euler(job.Rotation);
		}

		public static void SetPositionAndRotation(PoolJob.SpawnObjectJobParent job, GameObject objectToSpawn) {
			objectToSpawn.transform.parent = job.Parent;
		}

		public static void ResetPositionAndRotation(GameObject objectToSpawn) {
			objectToSpawn.transform.parent      = null;
			objectToSpawn.transform.position    = Vector3.zero;
			objectToSpawn.transform.rotation    = Quaternion.identity;
			objectToSpawn.transform.eulerAngles = Vector3.zero;
		}

		public static void SetPooledObjectActiveState(GameObject obj, bool state) {
			if (obj == null)
				return;

			obj.SetActive(state);
		}

		public static void DeactivatePooledObject(GameObject obj) {
			if (obj == null)
				return;

			obj.SetActive(false);
		}
	}
}