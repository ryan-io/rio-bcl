using System;

namespace BCL {
	public readonly struct InteractionCommand {
		public void PushToStack(IStack stack, Action callback) {
			stack.TieInToStack(callback);
		}
	}
}