using System;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityBCL {
	public static class NumberSeeding {
		const string Pool = "abcdefghijklmnopqrstuvwxyz0123456789";

		/// <summary>
		///     Creates a randomized string for use in procedural generation. This string can be fed back into this static
		///     class in order to obtain a hash code. This code is then responsible for seeding the entire generation process.
		/// </summary>
		/// <param name="seedLength">Default is 14 digits. Can be more or less.</param>
		/// <returns>A generated string for use as a guid</returns>
		/// <exception cref="Exception">Throw exception</exception>
		public static string CreateRandomSeed(int seedLength = 14) {
			if (seedLength <= 0) throw new Exception("Please enter an integer value greater than 0");

			var builder = new StringBuilder();
			for (var i = 0; i < seedLength; i++) {
				var index = Random.Range(0, Pool.Length);
				builder.Append(Pool[index]);
			}

			return builder.ToString();
		}

		public static int GetSeedHash(string seed) {
			var hash = seed.GetHashCode();

			return Mathf.Abs(hash);
		}

		public static string CreateRandomSeedStatic(int seedLength) {
			if (seedLength <= 0) throw new Exception("Please enter an integer value greater than 0");

			var builder = new StringBuilder();
			for (var i = 0; i < seedLength; i++) {
				var index = Random.Range(0, Pool.Length);
				builder.Append(Pool[index]);
			}

			return builder.ToString();
		}
	}
}