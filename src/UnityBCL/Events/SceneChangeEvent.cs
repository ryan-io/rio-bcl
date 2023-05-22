namespace UnityBCL {
	public readonly struct SceneChangeEvent {
		public const byte LoadStartByte    = 0;
		public const byte LoadCompleteByte = 1;

		public static SceneChangeEvent LoadStarted   => new(LoadStartByte);
		public static SceneChangeEvent LoadCompleted => new(LoadCompleteByte);

		public byte EventType { get; }

		public SceneChangeEvent(byte eventType) {
			EventType = eventType;
		}
	}
}