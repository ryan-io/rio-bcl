using System;

namespace BCL {
	/// <summary>
	///  Abstraction for a stack.
	/// </summary>
	public interface IStack {
		void TieInToStack(Action action);
	}
}