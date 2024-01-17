using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RIO.BCL {
	public class WeightedRandom<T> : IEnumerable {
		readonly           Random      _random;
		protected readonly List<Entry> Entries;
		double                         _accumulatedWeight;

		public WeightedRandom() {
			_random = new Random(1);
			Entries = new List<Entry>();
		}

		public WeightedRandom(int seed) {
			_random = new Random(seed);
			Entries = new List<Entry>();
		}

		public IEnumerator GetEnumerator() => Entries.GetEnumerator();

		public virtual void Add(T item, double weight) {
			_accumulatedWeight += weight;

			var entry = new Entry { Item = item, AccumulatedWeight = _accumulatedWeight };

			Entries.Add(entry);
		}

		public void AddRange(params Entry[]? entries) {
			if (entries == null || entries.Length <= 0)
				return;

			var count = entries.Length;

			for (var i = 0; i < count; i++)
				Add(entries[i].Item, entries[i].AccumulatedWeight);
		}

		public T? Pop() {
			var randomNumber = _random.NextDouble() * _accumulatedWeight;

			for (var i = 0; i < Entries.Count; i++)
				if (Entries[i].AccumulatedWeight >= randomNumber)
					return Entries[i].Item;

			return default;
		}

		public T Peek(T item) => Entries.Select(x => x.Item = item).FirstOrDefault();

		public bool Contains(T item) => Entries.Select(x => x.Item = item).FirstOrDefault() != null;

		public struct Entry {
			public T      Item;
			public double AccumulatedWeight;
		}
	}
}