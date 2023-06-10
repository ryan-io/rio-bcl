namespace Procedural {
	public static class GeneratorMessages {
		public const string InEditorOrNotPlaying = "Creating editor machine...";

		public const string MakingBuildMachine = "Creating build machine...";

		public const string NoObstacleLayer = "Obstacle layer does NOT exist... please create this layer.";

		public const string NoGroundLayer = "Ground layer does NOT exist... please create this layer.";

		public const string GenerationComplete = "Generation has completed... invoking events.";

		public const string GenerationStarting = "****Generation Starting****";

		public const string TimeToGenerate = "Total time to generate map: ";

		public const string Seconds = " seconds";

		public const string NoRoomDataAssigned =
			"Please assign a valid room data scriptable object. The generator is set to 'not generate' and requires pre-generated rooms to function.";
	}
}