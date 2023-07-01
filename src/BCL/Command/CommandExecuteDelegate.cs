using System;

namespace BCL {
	public class CommandExecuteDelegate : Command {
		readonly Action _action;

		public CommandExecuteDelegate(Action action) => _action = action;

		protected override void Run() {
			_action.Invoke();
		}
	}
}