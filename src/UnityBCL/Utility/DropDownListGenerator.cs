using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;

namespace UnityBCL {
	public struct DropDownListGenerator {
		const BindingFlags BindingFlagsConstStrings = BindingFlags.Public |
		                                              BindingFlags.Static |
		                                              BindingFlags.FlattenHierarchy;

		const BindingFlags BindingFlagsStaticStrings = BindingFlags.Public | BindingFlags.Static;

		public static IEnumerable CreateListConstStrings<T>() where T : struct {
			var dropDownList = new ValueDropdownList<object>();
			var structType   = typeof(T);
			var constantFields =
				structType.GetFields(BindingFlagsConstStrings)
				          .Where(f => f.IsLiteral) // For readonly, include f.IsInitOnly in addition
				          .ToList();

			for (var i = 0; i < constantFields.Count(); i++) {
				var text  = constantFields[i].GetRawConstantValue() as string;
				var value = constantFields[i].Name;
				dropDownList.Add(text, value);
			}

			return dropDownList;
		}

		public static IEnumerable CreateListStaticStrings<T>() where T : struct {
			var dropDownList = new ValueDropdownList<object>();
			var structType   = typeof(T);
			var constantFields =
				structType.GetFields(BindingFlagsStaticStrings)
				          .Where(f => f.FieldType == structType)
				          .ToDictionary(f => f.Name,
					           f => (string)f.GetValue(null));

			var list = new List<string>();

			foreach (var pair in constantFields)
				list.Add(constantFields[pair.Value]);

			var count = list.Count;

			for (var i = 0; i < count; i++)
				dropDownList.Add(list[i], list[i]);

			return dropDownList;
		}
	}
}