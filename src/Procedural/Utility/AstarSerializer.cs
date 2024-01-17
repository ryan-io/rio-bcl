using RIO.BCL.Serialization;
using Pathfinding.Serialization;
using UnityBCL;
using UnityBCL.Serialization;

namespace Procedural {
	public static class AstarSerializer {
		public const string Prefix = "AstarGraph_";

		/// <summary>
		///     Please ensure that your active graph data has been scanned.
		/// </summary>
		public static void SerializeCurrentAstarGraph(SerializerSetup setup, string name) {
			if (string.IsNullOrWhiteSpace(name)) name = "DefaultAstar";

			var settings = new SerializeSettings {
				nodes          = true,
				editorSettings = true
			};

			var serializer       = new Serializer();
			var bytes            = AstarPath.active.data.SerializeGraphs(settings);
		//	var serializationJob = new SerializeJob.Text(setup, name, bytes, setup.FileFormat);
			//serializer.SaveBytesData(serializationJob, true);
		}

		public static void DeserializeAstarGraph(AstarDeserializationJob job) {
			var serializer = new Serializer();
			var output     = job.DataPath + Prefix + job.NameOfMap + "_" + job.Seed + "_luid" + job.Iteration + ".txt";
			var hasData    = serializer.TryLoadBytesData(output, out var data);

			if (hasData) {
				AstarPath.active.data.DeserializeGraphs(data);
			}
			else {
				var log = new UnityLogging();
				log.Warning("Could not find serialized astar data with the output " + output);
			}
		}

		public readonly struct AstarDeserializationJob {
			public int    Iteration { get; }
			public string DataPath  { get; }
			public string Seed      { get; }
			public string NameOfMap { get; }

			public AstarDeserializationJob(string dataPath, string seed, int iteration, string nameOfMap) {
				DataPath  = dataPath;
				Seed      = seed;
				Iteration = iteration;
				NameOfMap = nameOfMap;
			}
		}
	}
}