using System;
using System.Collections;
using System.Collections.Generic;
using BCL.ExtensionMethods;
using BCL.Logging;

namespace BCL.Core {
	public interface ISignal {
		void Signal(bool constraintToSingle = false);
	}

	public interface ISignal<in T> {
		void Signal(T data, bool constraintToSingle = false);
	}

	public interface ISignal<in T1, in T2> {
		void Signal(T1 sender, T2 invokingObserver, bool constraintToSingle = false);
	}

	public interface IObservable {
		bool Register(Action observer);
		bool Unregister(Action observer);
		void UnregisterAll();
	}

	public interface IObservable<out T> {
		bool Register(Action<T> observer);
		bool Unregister(Action<T> observer);
		void UnregisterAll();
	}

	public interface IObservable<out T1, out T2> {
		bool Register(Action<T1, T2> observer);
		bool Unregister(Action<T1, T2> observer);
		void UnregisterAll();
	}

	public abstract class BaseObservable {
		protected bool ShouldLog { get; }

		protected ILogging? Logging { get; }

		protected BaseObservable() {
			ShouldLog = false;
		}

		protected BaseObservable(bool shouldLog) {
			ShouldLog = shouldLog;
		}

		protected BaseObservable(bool shouldLog, ILogging logging) {
			ShouldLog = shouldLog;
			Logging   = logging;
		}

		protected void HandleLogging() {
		}
	}

	public class Observable : BaseObservable, IEnumerable<Action>, IObservable, ISignal {
		public bool IsConstrained { get; private set; }

		public void Clear() => _observers.Clear();

		public IEnumerator GetEnumerator() {
			return _observers.GetEnumerator();
			// for (var i = 0; i < observers.Count(); i++)
			// 	yield return observers[i];
		}

		public bool Register(Action observer) {
			if (_observers.Contains(observer)) {
				return false;
			}

			try {
				_observers.Add(observer);
				return true;
			}

			catch (Exception e) {
				LogException(e);
				return false;
			}
		}

		public bool Unregister(Action observer) {
			if (_observers.IsNullOrEmpty() || !_observers.Contains(observer))
				return false;

			try {
				return _observers.Remove(observer);
			}
			catch (Exception e) {
				LogException(e);
				return false;
			}
		}

		public void Signal(bool constraintToSingle = false) {
			if (_observers.IsNullOrEmpty() || IsConstrained)
				return;

			foreach (var observer in _observers)
				observer.Invoke();

			IsConstrained = constraintToSingle;
		}

		public void UnregisterAll() => _observers.Clear();

		void LogException(Exception e) => Logging?.Log("Exception thrown.... " + e.Message);

		bool ContainsValue(Action observer) => _observers.Contains(observer);

		IEnumerator<Action> IEnumerable<Action>.GetEnumerator() => (IEnumerator<Action>)GetEnumerator();

		readonly HashSet<Action> _observers;

		public Observable() {
			_observers = new HashSet<Action>();
		}
	}

	public class Observable<T> : BaseObservable, IEnumerable<Action<T>>, IObservable<T>, ISignal<T> {
		public bool IsConstrained { get; private set; }

		public void Clear() => _observers.Clear();

		public IEnumerator GetEnumerator() => _observers.GetEnumerator();

		public bool Register(Action<T> observer) {
			if (_observers.Contains(observer)) {
				if (_observers.Contains(observer)) {
					if (ShouldLog)
						Logging?.Log(LogLevel.Warning, "Observers list already contains action. It will not be added.");
				}

				return false;
			}

			try {
				_observers.Add(observer);
				return true;
			}

			catch (Exception e) {
				LogException(e);
				return false;
			}
		}

		public bool Unregister(Action<T> observer) {
			if (_observers.IsNullOrEmpty() || !_observers.Contains(observer))
				return false;

			try {
				return _observers.Remove(observer);
			}
			catch (Exception e) {
				LogException(e);
				return false;
			}
		}

		public void Signal(T data, bool constraintToSingle = false) {
			if (_observers.IsNullOrEmpty() || IsConstrained)
				return;

			foreach (var observer in _observers)
				observer.Invoke(data);

			IsConstrained = constraintToSingle;
		}

		public void UnregisterAll() => _observers.Clear();

		void LogException(Exception e) => Logging?.Log(LogLevel.Error, "Exception thrown.... " + e.Message);

		bool ContainsValue(Action<T> observer) => _observers.Contains(observer);

		IEnumerator<Action<T>> IEnumerable<Action<T>>.GetEnumerator() => (IEnumerator<Action<T>>)GetEnumerator();

		public Observable() => _observers = new HashSet<Action<T>>();

		readonly HashSet<Action<T>> _observers;
	}

	public class Observable<T1, T2> : BaseObservable, IEnumerable<Action<T1, T2>>, IObservable<T1, T2>,
	                                  ISignal<T1, T2> {
		public bool IsConstrained { get; private set; }

		public void Clear() => _observers.Clear();

		readonly HashSet<Action<T1, T2>> _observers;

		public Observable() => _observers = new HashSet<Action<T1, T2>>();

		public void UnregisterAll() => _observers.Clear();

		IEnumerator<Action<T1, T2>> IEnumerable<Action<T1, T2>>.GetEnumerator() {
			return (IEnumerator<Action<T1, T2>>)GetEnumerator();
		}

		public IEnumerator GetEnumerator() => _observers.GetEnumerator();

		public bool Register(Action<T1, T2> observer) {
			if (_observers.Contains(observer)) return false;
			try {
				_observers.Add(observer);
				return true;
			}
			catch (Exception e) {
				LogException(e);
				return false;
			}
		}

		public bool Unregister(Action<T1, T2> observer) {
			if (!_observers.Contains(observer)) return false;
			try {
				return _observers.Remove(observer);
			}
			catch (Exception e) {
				LogException(e);
				return false;
			}
		}

		public void Signal(T1 sender, T2 invokingObserver, bool constraintToSingle = false) {
			try {
				if (_observers.IsNullOrEmpty() || IsConstrained)
					return;

				foreach (var observer in _observers)
					observer.Invoke(sender, invokingObserver);

				IsConstrained = constraintToSingle;
			}
			catch (Exception e) {
				LogException(e);
			}
		}

		bool ContainsValue(Action<T1, T2> observer) {
			return _observers.Contains(observer);
		}

		void LogException(Exception e) {
			Logging?.Log(LogLevel.Error, "Exception thrown.... " + e.Message);
		}
	}

	[Serializable]
	public class ObservableCollection<TKey> : Dictionary<TKey, Observable> where TKey : notnull {
	}

	[Serializable]
	public class ObservableCollection<TKey, T> : Dictionary<TKey, Observable<T>> where TKey : notnull {
	}

	[Serializable]
	public class ObservableCollection<TKey, T1, T2> : Dictionary<TKey, Observable<T1, T2>> where TKey : notnull {
	}

	[Serializable]
	public class ObservableCollectionMask<TKey> : Dictionary<TKey, IObservable> where TKey : notnull {
	}

	[Serializable]
	public class ObservableCollectionMask<TKey, T> : Dictionary<TKey, IObservable<T>> where TKey : notnull {
	}

	[Serializable]
	public class ObservableCollectionMask<TKey, T1, T2> : Dictionary<TKey, IObservable<T1, T2>> where TKey : notnull {
	}
}