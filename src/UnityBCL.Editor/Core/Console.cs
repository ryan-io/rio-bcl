using System.Reflection;

namespace UnityBCL.Editor {
	public readonly struct Console {
		public void Clear() {
			var editorAsm    = Assembly.GetAssembly(typeof(UnityEditor.Editor));
			var logEntryType = editorAsm.GetType("UnityEditor.LogEntries");
			var clearMethod  = logEntryType.GetMethod("Clear");
			clearMethod?.Invoke(new object(), null);
		}
	}
}