namespace BCL {
	public class AsyncRef<T> {
		public AsyncRef() {
		}

		public AsyncRef(T value) => Value = value;

		public T? Value { get; set; }

		public override string ToString() {
			var value = Value;
			return value == null ? "" : value.ToString();
		}

		public static implicit operator T?(AsyncRef<T?> r) => r.Value;

		public static implicit operator AsyncRef<T>(T value) => new(value);
	}
}