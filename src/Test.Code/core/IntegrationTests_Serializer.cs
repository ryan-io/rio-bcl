using System;
using RIO.BCL.Serialization;

namespace Test.Code {
	public class IntegrationTests_Serializer {
		public Info.Json Test_SerializeString() {
			var testString = "This is a tst string";
			var job        = new SerializeJob.Json("test-name213");
			var serializer = new Serializer();
			return serializer.SerializeAndSaveJson(testString, job);
		}

		public bool Test_EnsureDirectoryExists_FromUser(string folderName) {
			var rootPath = AppDomain.CurrentDomain.BaseDirectory + folderName;

			return Serializer.EnsureDirectoryExists(rootPath);
		}
	}
}