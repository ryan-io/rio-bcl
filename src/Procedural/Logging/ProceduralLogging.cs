using System;
using System.Globalization;
using UnityBCL;
using UnityEngine;

namespace Procedural {
	public class ProceduralLogging : MonoBehaviour {
		public const string GeneratedMeshBackground = "Generated Mesh Background";
		public const string GroundLayerName         = "Ground";
		public const string ObstaclesLayerName      = "Obstacles";

		public const string NoConfigOrShouldNotGen =
			"Generation at run time has been disabled. Now invoking OnGenerationComplete logic.";

		public const string LevelGeneratorTag = "LevelGenerator";

		static readonly UnityLogging Logger = new();

		public static void LogGeneratingMesh() {
#if UNITY_EDITOR || UNITY_STANDALONE
			Logger.Msg("Generating mesh", size: 15, italic: true, bold: true,
				ctx: $"{Strings.ProcGen} Mesh Generation");
#endif
		}

		public static void LogSettingTiles() {
#if UNITY_EDITOR || UNITY_STANDALONE
			Logger.Msg("Setting Tiles", size: 15, italic: true, bold: true,
				ctx: $"{Strings.ProcGen} TileMaps");
#endif
		}

		public static void LogProcessingMap() {
#if UNITY_EDITOR || UNITY_STANDALONE
			Logger.Msg("Processing map...", size: 15, italic: true, bold: true,
				ctx: $"{Strings.ProcGen} Map Processing");
#endif
		}

		public static void LogEndProcedural() {
#if UNITY_EDITOR || UNITY_STANDALONE
			Logger.Msg(
				"----------------------------------------------------------------------------" +
				"----------------------------------------> END PROCEDURAL GENERATION");
#endif
		}

		public static void LogStateChange<T>(Type stateId, T state, float timeInMilliseconds) where T : struct {
#if UNITY_EDITOR || UNITY_STANDALONE
			Logger.Msg(
				$"Changing '{stateId.Name}' state to:	'{state.ToString()}'						 {timeInMilliseconds / 1000} s. elapsed",
				bold: true, size: 14, ctx: $"{stateId.Name} State Change");
#endif
		}

		public static void LogScanComplete() {
#if UNITY_EDITOR || UNITY_STANDALONE
			Logger.Msg("Scan complete!");
#endif
		}

		public static void LogDisposing() {
#if UNITY_EDITOR || UNITY_STANDALONE
			Logger.Msg("Disposing...", "Post Generation Disposal");
#endif
		}

		public static void LogTotalTime(float totalTime, string unit) {
#if UNITY_EDITOR || UNITY_STANDALONE

			Logger.Msg(
				"Total generation time: " + $"{totalTime.ToString(CultureInfo.InvariantCulture)} {unit}",
				size: 15, italic: true, bold: true, ctx: $"{Strings.ProcGen} Post Generation");
#endif
		}
	}
}