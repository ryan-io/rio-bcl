using RIO.BCL;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace Procedural {
	[InlineEditor(InlineEditorObjectFieldModes.Foldout)]
	[CreateAssetMenu(menuName = "Procedural/Map Configuration", fileName = "procedural-map-configuration")]
	public class ProceduralMapConfiguration : ScriptableObject {
		[SerializeField] [ShowIf("UseRandomSeed", Value = Toggle.No)]
		public string Seed;

		[SerializeField] public string NameOfMap;

		[Title("Map Characteristics")] [SerializeField] [Range(3, 2500)]
		public int MapHeight = 100;

		[SerializeField] [Range(3, 2500)] public int MapWidth = 100;

		[SerializeField] [Range(1, 10)] public int MapBorderSize;

		[InfoBox("Recommended to leave 'CellSize' to a value of '1'", InfoMessageType.Error)]
		[ReadOnly]
		[SerializeField]
		[Range(1, 15)]
		public int CellSize = 1;

		[Title("Procedural Characteristics")] [SerializeField] [Range(1, 125)]
		public int SmoothingIterations = 5;

		[SerializeField] [Range(10, 1000)] public int WallRemovalThreshold = 50;

		[SerializeField] [Range(10, 1000)] public int RoomRemovalThreshold = 50;

		[SerializeField] [Range(1, 4)] public int LowerNeighborLimit = 4;

		[SerializeField] [Range(4, 8)] public int UpperNeighborLimit = 4;

		[SerializeField] [PropertyTooltip("Percentage of tiles that should be 'walls'")] [Range(40, 55)]
		public int WallFillPercentage;

		[SerializeField] [MinMaxSlider(1, 12)] public Vector2Int CorridorWidth = new(1, 6);

		[Title("Layer Masks")] [SerializeField]
		public LayerMask GroundLayerMask;

		[SerializeField] public LayerMask ObstacleLayerMask;

		[SerializeField] public LayerMask BoundaryLayerMask;

		[Title("Seeding")] [SerializeField] [EnumToggleButtons]
		public Toggle UseRandomSeed;

		public int2 MapSizeInt => new(MapWidth, MapHeight);

		public Vector2Int MapSizeVector {
			get {
				var width  = CellSize * MapWidth;
				var height = CellSize * MapHeight;
				return new Vector2Int(width, height);
			}
		}
	}
}