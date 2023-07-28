// Test.Code

namespace Test.Code {
	public class MarshalObject {
		public int[,] CoreMap   { get; set; }
		public int[,] BorderMap { get; set; }

		public MarshalObject(int width, int height, int borderSize) {
			CoreMap = new int[width, height];
			BorderMap = new int[
				width  + borderSize * 2,
				height + borderSize * 2];
		}
	}
}