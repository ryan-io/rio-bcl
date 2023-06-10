using System.Collections.ObjectModel;
using System.Linq;

namespace Curves {
	public class IndexBuilder {
		readonly int   _arraySize;
		readonly int   _increment;
		readonly int[] _rootIndexArrayOrigin;

		int[] _rootIndexArray;

		public IndexBuilder(int arraySize, int increment) {
			_arraySize = arraySize;
			_increment = increment;

			_rootIndexArrayOrigin = Build();
		}

		public void Increment() {
			for (var i = 0; i < _rootIndexArray.Length; i++)
				_rootIndexArray[i] += _increment;
		}

		public void Decrement() {
			for (var i = 0; i < _rootIndexArray.Length; i++)
				_rootIndexArray[i] -= _increment;
		}

		public void Reset()
			=> _rootIndexArray = new int[_arraySize];

		public ReadOnlyCollection<int> GetIndexArray()
			=> _rootIndexArray.ToList().AsReadOnly();

		public ReadOnlyCollection<int> GetOriginArray()
			=> _rootIndexArrayOrigin.ToList().AsReadOnly();

		int[] Build() {
			Reset();

			for (var i = 0; i < _rootIndexArray.Length; i++)
				_rootIndexArray[i] = i;

			return _rootIndexArray;
		}
	}
}