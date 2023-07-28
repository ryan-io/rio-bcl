// Test.Code

using System;

namespace Test.Code {
	public readonly ref struct SpanWrapper {
		public Span<int> Span { get; }
		public Span<int> SpanX { get; }
		public Span<int> SpanY { get; }
		public Span<int> SpanZ { get; }

		public SpanWrapper(Span<int> span, Span<int> spanX, Span<int> spanY, Span<int> spanZ) {
			Span       = span;
			SpanX      = spanX;
			SpanY      = spanY;
			SpanZ = spanZ;
		}
	}
}