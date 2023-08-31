using System;

namespace BCL {
	public class Singleton<T> where T : class, new() {
		protected ILogging? InternalLogging { get; set; }

		public void InjectLogger(ILogging logger) => InternalLogging = logger;
		
		public static T Instance {
			get {
				lock (LockObj) 
					return _instance.Value;
			}
		}

		static readonly object LockObj = new();

		static readonly Lazy<T> _instance = new (() => new T());
		
		public Singleton() {
			
		}
	}
}