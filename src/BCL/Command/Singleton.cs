using System;

namespace BCL {
	public class Singleton<T> where T : class, new() {
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