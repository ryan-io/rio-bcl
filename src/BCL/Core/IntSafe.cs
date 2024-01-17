namespace RIO.BCL {
	public struct IntSafe {
		/// <summary>
		///  Thread safe value of type integer.
		/// </summary>
		public int Value {
			get => _value;
			private set {
				lock (_mutex) {
					_value = value;
				}
			}
		}

		/// <summary>
		///  The value that is protected by the mutex. Backing field to Value.
		/// </summary>
		int _value;

		/// <summary>
		///  Increment operator
		/// </summary>
		/// <param name="safe">this* IntSafe</param>
		/// <returns>IntSafe&</returns>
		public static IntSafe operator ++(IntSafe safe) {
			safe.Value++;
			return safe;
		}

		/// <summary>
		///  Decrement operator
		/// </summary>
		/// <param name="safe">this* IntSafe</param>
		/// <returns>IntSafe&</returns>
		public static IntSafe operator --(IntSafe safe) {
			safe.Value--;
			return safe;
		}

		/// <summary>
		///  Implicit conversion from IntSafe to string.
		/// </summary>
		/// <returns>string object</returns>
		public override string ToString() => Value.ToString();

		/// <summary>
		///  Constructor that takes an optional start value.
		/// </summary>
		/// <param name="start"></param>
		public IntSafe(int start = 0) {
			_mutex = new();
			_value = 0;
			Value  = start;
		}

		/// <summary>
		///  Protects Value from being changed by multiple threads.
		/// </summary>
		readonly object _mutex;
	}
}