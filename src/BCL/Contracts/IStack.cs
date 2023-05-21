using System;

namespace BCL {
	public interface IStack {
		void TieInToStack(Action action);
	}
}