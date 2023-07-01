using System;
using Cysharp.Threading.Tasks;
using UnityBCL;
using UnityEngine;

namespace Procedural {
	public abstract class ColliderSolver {
		public abstract UniTask CreateCollider(ColliderSolverModel model);

		protected GameObject AddRoom(GameObject parent, string identifier = "", params Type[] componentsToAdd) {
			var newObj = new GameObject($"Room {identifier} - Colliders") {
				transform = {
					parent = parent.transform
				}
			};

			if (!componentsToAdd.IsEmptyOrNull())
				foreach (var component in componentsToAdd)
					newObj.AddComponent(component);

			return newObj;
		}
	}
}