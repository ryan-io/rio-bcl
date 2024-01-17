using System;

namespace RIO.BCL {
	/// <summary>
	///  Abstraction for a stack.
	/// </summary>
	public interface IStack {
		void TieInToStack(Action action);
	}
}