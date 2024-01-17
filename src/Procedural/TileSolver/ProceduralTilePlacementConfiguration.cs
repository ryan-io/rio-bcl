using RIO.BCL;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Procedural {
	[InlineEditor(InlineEditorObjectFieldModes.Foldout)]
	[CreateAssetMenu(menuName = "Procedural/Tile Configuration", fileName = "procedural-tile-configuration")]
	public class ProceduralTilePlacementConfiguration : ScriptableObject {
		[SerializeField] [TitleGroup("Primary Tiles", HorizontalLine = false)]
		TileBase _ground;

		[SerializeField] [TitleGroup("Primary Tiles", HorizontalLine = false)]
		TileBase _boundary;

		[SerializeField] [TitleGroup("Angle Tiles", HorizontalLine = false)]
		TileBase _angleNorthEast;

		[SerializeField] [TitleGroup("Angle Tiles", HorizontalLine = false)]
		TileBase _angleNorthWest;

		[SerializeField] [TitleGroup("Angle Tiles", HorizontalLine = false)]
		TileBase _angleSouthEast;

		[SerializeField] [TitleGroup("Angle Tiles", HorizontalLine = false)]
		TileBase _angleSouthWest;

		[SerializeField] [TitleGroup("Pocket Tiles", HorizontalLine = false)]
		TileBase _pocketNorth;

		[SerializeField] [TitleGroup("Pocket Tiles", HorizontalLine = false)]
		TileBase _pocketSouth;

		[SerializeField] [TitleGroup("Pocket Tiles", HorizontalLine = false)]
		TileBase _pocketEast;

		[SerializeField] [TitleGroup("Pocket Tiles", HorizontalLine = false)]
		TileBase _pocketWest;

		[SerializeField] [EnumToggleButtons] [TitleGroup("Debugging", HorizontalLine = false)]
		readonly Toggle _createLabels = Toggle.No;

		[SerializeField] [EnumToggleButtons] [TitleGroup("Angle Tiles", HorizontalLine = false)]
		readonly Toggle _generateAngles = Toggle.Yes;

		public bool ShouldCreateLabels
			=> _createLabels == Toggle.Yes;

		public bool ShouldGenerateAngles
			=> _generateAngles == Toggle.Yes;

		public TileBase Ground
			=> _ground;

		public TileBase Boundary
			=> _boundary;

		public TileBase AngleNorthEast
			=> _angleNorthEast;

		public TileBase AngleNorthWest
			=> _angleNorthWest;

		public TileBase AngleSouthEast
			=> _angleSouthEast;

		public TileBase AngleSouthWest
			=> _angleSouthWest;

		public TileBase PocketNorth
			=> _pocketNorth;

		public TileBase PocketSouth
			=> _pocketSouth;

		public TileBase PocketEast
			=> _pocketEast;

		public TileBase PocketWest
			=> _pocketWest;
	}
}