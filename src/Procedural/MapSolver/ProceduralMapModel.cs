using UnityEngine;

namespace Procedural {
	public class ProceduralMapModel {
		public GameObject Background { get; set; }
		public int[,]     Map        { get; set; }
		public int[,]     BorderMap  { get; set; }
		public int        SeedValue  { get; set; }
	}
}