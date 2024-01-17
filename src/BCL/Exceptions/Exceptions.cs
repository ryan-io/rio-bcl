using System;

namespace RIO.BCL {
	public class NoBitmapParserException : Exception {
		public override string Message => "Cannot save bitmaps. No bitmap parser has been assigned.";
	}

	public class BitMapParserDisposedException : Exception {
		public override string Message
			=> "Instance of BitmapParser has been disposed. No further actions can be taken. Please create a new parser.";
	}

	public class ImageGrabberNotPrimedException : Exception {
		public override string Message
			=> "An instance of ImageGrabber exists, but no file directory has been provided.";
	}

	public class NoImageParserAssignedException : Exception {
		public override string Message => "No image parser has been assigned";
	}
}