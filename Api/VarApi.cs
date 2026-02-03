using Luny;
using LunyScript.Blocks;
using System;

namespace LunyScript.Api
{
	public readonly struct VarApi
	{
		private readonly Table _table;

		internal VarApi(Table table) => _table = table;

		public ScriptVariable Get(String name) => ScriptVariable.From(_table.GetHandle(name));

		public IScriptActionBlock Set(String name, Variable value) => Get(name).Set(value);
		public IScriptActionBlock Set(String name, IScriptVariable value) => Get(name).Set(value);

		public IScriptActionBlock Add(String name, Variable value) => Get(name).Add(value);
		public IScriptActionBlock Add(String name, IScriptVariable value) => Get(name).Add(value);

		public IScriptConditionBlock IsTrue(String name) => Get(name).IsTrue();
		public IScriptConditionBlock IsFalse(String name) => Get(name).IsFalse();

		// public Variable this[String variableName]
		// {
		// 	get => _table[variableName];
		// 	set => _table[variableName] = value;
		// }
	}
}
