using System;
using BCL;
using UnityBCL;

namespace Procedural {
	public class ProceduralExitHandler {
		const string OnQuitId = "onquit";

		readonly ObservablesCollection _observables;

		public ProceduralExitHandler()
			=> _observables = new ObservablesCollection(true) { { OnQuitId, new Observable() } };

		public IObservable OnQuit => _observables[OnQuitId];

		public bool DetermineQuit(Func<bool> statement, bool fireEventsIfTrue = true) {
			var shouldQuit = statement.Invoke();

			if (shouldQuit && fireEventsIfTrue)
				_observables[OnQuitId]?.Signal();

			return shouldQuit;
		}

		public bool DetermineQuit(params Func<bool>[] statements) {
			if (statements.IsEmptyOrNull())
				return false;

			foreach (var statement in statements) {
				var shouldQuit = DetermineQuit(statement);

				if (shouldQuit)
					//					Logger.Warning("An exit statement was found to be true.");
					return true;
			}

			return false;
		}
	}
}