using UnityEngine;

namespace UnityBCL {
	public abstract class SingletonMonobehavior<TComponent> : MonoBehaviour where TComponent : MonoBehaviour {
		static readonly object      Lock = new();
		static          TComponent? _instanceComponent;
		static          bool        _isShuttinDown;

		public static TComponent? Global {
			get {
				if (_isShuttinDown)
					return default;

				var instanceComponent = InstanceComponent;
				return _instanceComponent ??= instanceComponent != null
					                              ? instanceComponent.GetComponent<TComponent>()
					                              : null;
			}
		}

		static TComponent? InstanceComponent {
			get {
				if (_isShuttinDown)
					return
						null;

				lock (Lock) {
					if (_instanceComponent is null) {
						// Try to get existing Instance
						_instanceComponent = (TComponent)FindObjectOfType(typeof(TComponent));

						if (_instanceComponent == null) {
							var singleton = new GameObject();
							_instanceComponent = singleton.AddComponent<TComponent>();
							singleton.name     = typeof(TComponent) + " Singleton";

							DontDestroyOnLoad(singleton);
						}
					}
				}

				return _instanceComponent;
			}
		}

		void OnDestroy() {
			_isShuttinDown = true;
		}

		void OnApplicationQuit() {
			_isShuttinDown = true;
		}
	}

	public abstract class Singleton<Tmonocomp, Tinterface> : MonoBehaviour
		where Tmonocomp : MonoBehaviour
		where Tinterface : class {
		static          bool       _shuttingDown;
		static readonly object     _lock              = new();
		static          Tmonocomp  _instanceComponent = null!;
		static          Tinterface _instanceInterface = null!;

		public static Tinterface Global {
			get {
				if (_shuttingDown) return null!;
				var instanceComponent = InstanceComponent;

				if (_instanceInterface is null) _instanceInterface = instanceComponent.GetComponent<Tinterface>();

				return _instanceInterface;
			}
		}

		static Tmonocomp InstanceComponent {
			get {
				if (_shuttingDown) return null!;

				lock (_lock) {
					if (_instanceComponent is null) {
						// Try to get existing Instance
						_instanceComponent = (Tmonocomp)FindObjectOfType(typeof(Tmonocomp));

						if (_instanceComponent is null) {
							var singleton = new GameObject();
							_instanceComponent = singleton.AddComponent<Tmonocomp>();
							singleton.name     = typeof(Tmonocomp) + " Singleton";

							DontDestroyOnLoad(singleton);
						}
					}
				}

				return _instanceComponent;
			}
		}

		void OnDestroy() {
			_shuttingDown = true;
		}

		void OnApplicationQuit() {
			_shuttingDown = true;
		}
	}
}