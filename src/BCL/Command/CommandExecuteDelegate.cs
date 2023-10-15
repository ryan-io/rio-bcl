using System;

namespace BCL {
	/// <summary>
	/// Non generic command that executes an action.
	/// </summary>
	public class CommandExecuteDelegate : Command {
		readonly Action _action;

		public CommandExecuteDelegate(Action action) => _action = action;

		protected override void Run() {
			_action.Invoke();
		}
	}
}