using System;

namespace RIO.BCL {
	/// <summary>
	///  Command that pushes a callback to a stack.
	/// </summary>
	public readonly struct CommandPushToStack {
		public void PushToStack(IStack stack, Action callback) {
			stack.TieInToStack(callback);
		}
	}
}