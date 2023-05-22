using BCL;
using UnityEngine;

namespace UnityBCL {
	public class AsyncColliderOpCallbacks : GenericCollection<IAsyncOpCallback<Collider>> {
		protected override IAsyncOpCallback<Collider> CreateNewMember() {
			return new AsyncOpCallback<Collider>();
		}
	}
}