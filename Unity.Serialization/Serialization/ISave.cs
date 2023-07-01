using UnityEngine;

namespace Unity.Serialization.Serialization {
	public interface ISave {
		void Save(Object obj, string fileName, bool overWrite = false);
	}
}