using System;
using BCL.Contracts;

namespace BCL.Command {
	public readonly struct InteractionCommand {
		public void PushToStack(IStack stack, Action callback) {
			stack.TieInToStack(callback);
		}
	}
}