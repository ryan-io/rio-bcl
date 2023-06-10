using System;

namespace BCL {
	public abstract class CommandRunAction : Command<CommandRunAction.Args> {
		readonly Action _action;

		public CommandRunAction(Action action) => _action = action;

		protected override void InternalExecute(Args args) {
			_action.Invoke();
		}

		public readonly struct Args {
		}
	}
}