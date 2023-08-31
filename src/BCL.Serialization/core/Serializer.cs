using System;
using System.IO;
using Newtonsoft.Json;

namespace BCL.Serialization {
	public class Serializer {
		ILogging? Log { get; }

		public Info.Json SerializeAndSaveJson(object obj, SerializeJob.Json job, bool sanitizeName = true,
			bool dontOverwrite = true) {
			try {
				var name = job.SaveName;

				if (sanitizeName)
					name = InternalSanitizeName(job.SaveName);

				name += JSON_FILE;

				var saveLocation = GetSaveLocation(job);
				saveLocation += BACKSLASH;

				EnsureDirectoryExists(saveLocation);

				var path       = saveLocation + name;
				var fileExists = File.Exists(path);

				if (dontOverwrite) {
					if (fileExists) {
						return new Info.Json(path, true, false);
					}
				}

				var json = JsonConvert.SerializeObject(obj, Formatting.Indented);

				File.WriteAllText(path, json);
				return new Info.Json(path, true, fileExists);
			}
			catch (Exception e) {
				Log?.Log(LogLevel.Error, e.Message);
				return new Info.Json(string.Empty, false, false);
			}
		}

		string GetSaveLocation(SerializeJob.Json job) {
			return !string.IsNullOrWhiteSpace(job.SavePath) ? job.SavePath : _defaultSaveLocation;
		}

		public T? DeserializeJson<T>(string jsonFilePath) {
			if (string.IsNullOrWhiteSpace(jsonFilePath))
				return default;

			var hasFile = File.Exists(jsonFilePath);

			if (!hasFile)
				return default;

			var content = File.ReadAllText(jsonFilePath);
			return JsonConvert.DeserializeObject<T>(content);
		}

		public void SerializeAndSaveBytesTxt(SerializeJob.Text job, bool sanitizeName) {
			var name = job.SaveName;

			var location = job.SavePath + BACKSLASH + name + TXT_FILE;

			File.WriteAllBytes(location, job.Data);
		}

		public bool TryLoadBytesData(string filePathFull, out byte[]? data) {
			var exists = File.Exists(filePathFull);

			if (!exists) {
				Log?.Log(LogLevel.Error, $"Could not find a file at: {filePathFull}");
				data = default;
				return false;
			}

			data = File.ReadAllBytes(filePathFull);
			return true;
		}

		public static bool EnsureDirectoryExists(string folderName, string rootPath) {
			var path = rootPath + BACKSLASH + folderName;
			return EnsureDirectoryExists(path);
		}

		public static bool EnsureDirectoryExists(string path) {
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
				return false;
			}

			return true;
		}

		public void EnsureFileExists(string path, string name, string format) {
			if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(name) ||
			    string.IsNullOrWhiteSpace(format)) {
				Log?.Log("String passed to EnsureFileExists was null.");
				return;
			}

			var fullPath = path + BACKSLASH + name + format;
			var exists   = File.Exists(fullPath);

			if (!exists) {
				using var fs = File.Create(fullPath);
				fs.Flush();
			}

			Log?.Log($"Check fil at : {fullPath} - {exists}");
		}

		public string InternalSanitizeName(string value) {
			var sanitizedString = Sanitizer.Sanitize(value);
			return sanitizedString == string.Empty ? "stringWasEmpty" : sanitizedString;
		}

		public Serializer() {
		}

		public Serializer(ILogging log) {
			Log = log;
		}

		const string JSON_FILE = ".json";
		const string TXT_FILE  = ".txt";
		const string BACKSLASH = "/";

		readonly        string _defaultSaveLocation = AppDomain.CurrentDomain.BaseDirectory + DefaultFolder;
		static readonly string DefaultFolder        = "serialized-data";
	}
}