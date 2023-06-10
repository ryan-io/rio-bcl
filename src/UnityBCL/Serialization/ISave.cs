using UnityEngine;

namespace UnityBCL {
	public interface ISave {
		void Save(Object obj, string fileName, bool overWrite = false);
	}
}