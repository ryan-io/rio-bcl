using System;

namespace BCL {
	public readonly struct CommandPushToStack {
		public void PushToStack(IStack stack, Action callback) {
			stack.TieInToStack(callback);
		}
	}
}