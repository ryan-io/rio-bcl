using System;
using System.Collections.Generic;

namespace BCL {
	public class CommandExecuteStack : LinkedList<CommandExecuteDelegate> {
		readonly CommandExecuteDelegate _rootCommand;

		bool _isInitialized;

		public CommandExecuteStack(CommandExecuteDelegate rootCommand) {
			_rootCommand = rootCommand;
			IsValid      = true;
		}

		/// <summary>
		///     Returns true if a rootCommand has been passed to constructor. Returns false if rootCommand is not assigned.
		/// </summary>
		public bool IsValid { get; }

		public void Initialize() {
			_isInitialized = true;
			Clear();

			AddFirst(_rootCommand);
		}

		public void Next() {
			if (!_isInitialized) return;

			PopNext().Execute();
		}

		public void Push(CommandExecuteDelegate cmd) {
			if (!_isInitialized)
				return;

			AddLast(cmd);
		}

		CommandExecuteDelegate PopNext() {
			if (!_isInitialized)
				throw new Exception("Stack is not initialized. This requires debugging");

			if (Count < 1)
				AddFirst(_rootCommand);

			var nextPop = Last.Value;
			RemoveLast();

			return nextPop;
		}
	}
}