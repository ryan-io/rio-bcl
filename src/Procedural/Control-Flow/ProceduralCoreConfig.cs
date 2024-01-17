using RIO.BCL;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Procedural {
	[InlineEditor(InlineEditorObjectFieldModes.Foldout)]
	[CreateAssetMenu(menuName = "Procedural/Core Configuration", fileName = "procedural-core-configuration")]
	public class ProceduralCoreConfig : SerializedScriptableObject {
		[field: SerializeField]
		[field: Range(1, 100)]
		[field: Title("General")]
		public int FramesToDelayBeforeGeneration { get; private set; } = 10;

		[field: SerializeField]
		[field: EnumToggleButtons]
		[field: Title("State")]
		public ApplicationState ApplicationState { get; private set; } = ApplicationState.Editor;

		[field: SerializeField]
		[field: EnumToggleButtons]
		public RuntimeState RuntimeState { get; set; } = RuntimeState.Generate;

		[field: SerializeField]
		[field: EnumToggleButtons]
		[field: Title("Gizmos")]
		public Toggle DrawDebugGizmos { get; private set; } = Toggle.No;
	}
}