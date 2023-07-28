using UnityEngine;

namespace UnityBCL.Serialization {
	public interface ISave {
		void Save(Object obj, string fileName, bool overWrite = false);
	}
}