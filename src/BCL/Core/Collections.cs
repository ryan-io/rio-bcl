using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BCL {
	public class StringCollection : GenericCollection<string> {
		public StringCollection(bool allowAutoCreation) : base(allowAutoCreation) {
		}

		protected override string CreateNewMember() => string.Empty;
	}

	public class ObservablesCollection : GenericCollection<IObservable> {
		public ObservablesCollection(bool allowAutoCreateInstance) : base(allowAutoCreateInstance) {
		}

		protected override IObservable CreateNewMember() => new Observable();
	}

	public abstract class GenericCollection<T> : IEnumerable<T> {
		readonly bool _allowAutoCreateInstance;

		readonly HashSet<GenericCollectionMember<T>> _members;

		protected GenericCollection(bool allowAutoCreateInstance = true) {
			_members                 = new HashSet<GenericCollectionMember<T>>();
			_allowAutoCreateInstance = allowAutoCreateInstance;
		}

		public T this[string name] {
			get => Find(name);
			set => Add(name, value);
		}

		public IEnumerator<T> GetEnumerator() {
			foreach (var member in _members)
				yield return member.Member;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Add(string name) => Add(name, (_allowAutoCreateInstance ? CreateNewMember() : default)!);

		public void AddRange(params string[] names) {
			foreach (var name in names) Add(name);
		}

		public void Clear(string identifier) {
			if (string.IsNullOrWhiteSpace(identifier))
				return;

			var o = FirstOrDefault(identifier);
			_members.Remove(o);
		}

		public void ClearAll() => _members?.Clear();

		protected abstract T CreateNewMember();

		T Find(string name) {
			var collection = FirstOrDefault(name);

			if (!string.IsNullOrWhiteSpace(collection.Id))
				return collection.Member;

			if (_allowAutoCreateInstance)
				collection = new GenericCollectionMember<T>(name, CreateNewMember());

			return collection.Member;
		}

		public void Add(string name, T member) {
			if (Contains(name))
				return;

			var collection = new GenericCollectionMember<T>(name, member);
			_members.Add(collection);
		}

		public bool Contains(string identifier)
			=> _members.Select(x => x.Id == identifier).FirstOrDefault();

		GenericCollectionMember<T> FirstOrDefault(string identifier)
			=> _members.FirstOrDefault(x => x.Id.Equals(identifier, StringComparison.OrdinalIgnoreCase));
	}

	internal readonly struct GenericCollectionMember<T> {
		public string Id     { get; }
		public T      Member { get; }

		public GenericCollectionMember(string id, T member) {
			Id     = id;
			Member = member;
		}
	}
}