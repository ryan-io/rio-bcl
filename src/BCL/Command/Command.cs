namespace BCL {
	public abstract class Command<T> {
		public void Execute(T args) {
			if (!ValidateArguments(args))
				return;

			Run(args);
		}

		protected abstract void Run(T args);

		static bool ValidateArguments(T args) => args?.GetType() == typeof(T);
	}

	public abstract class Command {
		public void Execute() {
			Run();
		}

		protected abstract void Run();
	}
}