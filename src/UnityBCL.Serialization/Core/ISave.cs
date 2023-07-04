using UnityEngine;

namespace UnityBCL.Serialization.Core {
	public interface ISave {
		void Save(Object obj, string fileName, bool overWrite = false);
	}
}