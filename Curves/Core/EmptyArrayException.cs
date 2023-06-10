using System;

namespace Curves {
	public class EmptyArrayException : Exception {
		public override string Message =>
			"No points have been added to this file. Please add points and attempt to loagd again";
	}
}