using System.Collections;
using OmniBCL.ExtensionMethods;
using OmniBCL.Logging;
using UnityEngine;
using ILogger = OmniBCL.Logging.ILogger;

namespace OmniBCL.Core;

public interface IObservable {
	bool Register(Action observer);
	bool Unregister(Action observer);
	void UnregisterAll();
	void Signal(bool constraintToSingle = false);
}

public interface IObservable<out T> {
	bool Register(Action<T> observer);
	bool Unregister(Action<T> observer);
}

public interface IObservable<out T1, out T2> {
	bool Register(Action<T1, T2> observer);
	bool Unregister(Action<T1, T2> observer);
}

public abstract class BaseObservable {
	protected bool ShouldLog { get; }

	protected ILogger? Logger { get; }

	protected BaseObservable() {
		ShouldLog = false;
	}

	protected BaseObservable(bool shouldLog) {
		ShouldLog = shouldLog;

		if (shouldLog)
			Logger = new UnityLogging(this);
	}

	protected void HandleLogging() {
	}
}

public class Observable : BaseObservable, IEnumerable<Action>, IObservable {
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

	void LogException(Exception e) => Debug.Log("Exception thrown.... " + e.Message);

	bool ContainsValue(Action observer) => _observers.Contains(observer);

	IEnumerator<Action> IEnumerable<Action>.GetEnumerator() => (IEnumerator<Action>)GetEnumerator();

	readonly HashSet<Action> _observers;

	public Observable() {
		_observers = new HashSet<Action>();
	}
}

public class Observable<T> : BaseObservable, IEnumerable<Action<T>>, IObservable<T> {
	public bool IsConstrained { get; private set; }

	public void Clear() => _observers.Clear();

	public IEnumerator GetEnumerator() => _observers.GetEnumerator();

	public bool Register(Action<T> observer) {
		if (_observers.Contains(observer)) {
			if (_observers.Contains(observer)) {
				if (ShouldLog)
					Logger?.Log(LogLevel.Warning,"Observers list already contains action. It will not be added.");
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

	void LogException(Exception e) => Debug.Log("Exception thrown.... " + e.Message);

	bool ContainsValue(Action<T> observer) => _observers.Contains(observer);

	IEnumerator<Action<T>> IEnumerable<Action<T>>.GetEnumerator() => (IEnumerator<Action<T>>)GetEnumerator();

	public Observable() => _observers = new HashSet<Action<T>>();

	readonly HashSet<Action<T>> _observers;
}

public class Observable<T1, T2> : IEnumerable<Action<T1, T2>>, IObservable<T1, T2> {
	public bool IsConstrained { get; private set; }

	public void Clear() => _observers.Clear();

	readonly HashSet<Action<T1, T2>> _observers;

	public Observable() => _observers = new HashSet<Action<T1, T2>>();

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
		Debug.Log("Exception thrown.... " + e.Message);
	}
}