using System;

namespace RIO.BCL {
	/// <summary>
	///   A singleton class. Provides a single static instance of class type T.
	///  <para>Usage: public class MyClass : Singleton&lt;MyClass&gt; { }</para>
	///  Can be accessed from anywhere using MyClass.Instance
	///  Can also inject a logger using InjectLogger(ILogging logger); this implementation of logger is internal
	///  to riot-bcl.
	/// </summary>
	/// <typeparam name="T">Type instance create a Singleton for</typeparam>
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