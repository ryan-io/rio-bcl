namespace BCL {
	public abstract class Command<T> {
		public void Execute(T args) {
			if (!ValidateArguments(args))
				return;

			InternalExecute(args);
		}

		protected abstract void InternalExecute(T args);

		static bool ValidateArguments(T args) => args?.GetType() == typeof(T);
	}
}