using System.Collections.Generic;
using Pathfinding;
using UnityBCL;

namespace Procedural {
	public class GridGraphRuleRemover {
		public void Remove(GridGraph gridGraph) {
			var currentRules = new List<GridGraphRule>(gridGraph.rules.GetRules());

			if (!currentRules.IsNullOrEmpty())
				foreach (var rule in currentRules)
					gridGraph.rules.RemoveRule(rule);
		}
	}
}