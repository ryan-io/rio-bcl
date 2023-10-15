namespace BCL {
	/// <summary>
	/// Simple implementation of the command pattern. Generic version.
	/// </summary>
	/// <typeparam name="T">Generic type, T; your choice</typeparam>
	public abstract class Command<T> {
		public void Execute(T args) {
			if (!ValidateArguments(args))
				return;

			Run(args);
		}

		protected abstract void Run(T args);

		static bool ValidateArguments(T args) => args?.GetType() == typeof(T);
	}

	/// <summary>
	///  Simple implementation of the command pattern. Non-generic version.
	/// </summary>
	public abstract class Command {
		public void Execute() {
			Run();
		}

		protected abstract void Run();
	}
}