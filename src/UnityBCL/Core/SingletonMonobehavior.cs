using UnityEngine;

namespace UnityBCL.Core {
	public abstract class SingletonMonobehavior<TComponent> : MonoBehaviour where TComponent : MonoBehaviour {
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

		static readonly object      Lock = new();
		static          TComponent? _instanceComponent;
		static          bool        _isShuttinDown;
	}

	public abstract class Singleton<TComponent, TInterface> : MonoBehaviour where TComponent : MonoBehaviour
	                                                                        where TInterface : class {
		public static TInterface Global {
			get {
				if (_isShuttinDown)
					return default!;

				var instanceComponent = InstanceComponent;
				return (_instanceInterface ??= instanceComponent != null
					                               ? instanceComponent.GetComponent<TInterface>()
					                               : null)!;
			}
		}

		static TComponent InstanceComponent {
			get {
				if (_isShuttinDown)
					return null!;

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

		static readonly object      Lock = new();
		static          TComponent? _instanceComponent;
		static          TInterface? _instanceInterface;
		static          bool        _isShuttinDown;
	}
}