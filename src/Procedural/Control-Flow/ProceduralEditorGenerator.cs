#if UNITY_EDITOR || UNITY_STANDALONE
using Sirenix.OdinInspector;
using UnityBCL;
using UnityEngine;

namespace Procedural {
	public class ProceduralEditorGenerator : Singleton<ProceduralEditorGenerator, ProceduralEditorGenerator> {
		[Title("Required Monobehaviors")] [SerializeField] [Required]
		ProceduralController _controller;

		[SerializeField] [PropertySpace(0, 10)] [Required]
		ProceduralMapStateMachine _stateMachine;

		bool ShouldFix => !_controller || !_stateMachine;

		[Button]
		[ShowIf("@ShouldFix")]
		void Fix() {
			_controller   = gameObject.FixComponent<ProceduralController>();
			_stateMachine = gameObject.FixComponent<ProceduralMapStateMachine>();
		}

		[Button]
		[TabGroup("1", "Procedural Generation Control")]
		async void GenerateMap() {
			_stateMachine.ForceAllInitialize();
			_controller.ResetToken();
			_controller.InitializeGeneration();
			await _controller.BeginGeneration();
		}

		[Button(ButtonStyle.CompactBox)]
		[TabGroup("1", "Procedural Generation Control")]
		void ResetMap() {
			_controller.ResetToken();
			foreach (var component in _controller.GetFlowComponentsAsCreation)
				component.Dispose(_controller.CancellationToken);
		}

		[Button(ButtonStyle.CompactBox)]
		[TabGroup("1", "Procedural Generation Control")]
		public void CancelGeneration() => _controller.ResetToken(false);
	}
}

#endif