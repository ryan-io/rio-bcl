using System;

namespace BCL.Contracts {
	public interface IStack {
		void TieInToStack(Action action);
	}
}