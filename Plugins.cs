using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Reflection;

namespace Snowbull.API {
	internal class Plugins<T> where T : class, API.IContext {
		private Type[] Types {
			get;
			set;
		}

		internal Plugins(Type[] types) {
			Types = types;
		}

		internal Plugin<T>[] Initialise() {
			Plugin<T>[] plugins = new Plugin<T>[Types.Length];
			for(int i = 0; i < Types.Length; i++) plugins[i] = (Plugin<T>) Activator.CreateInstance(Types[i]);
			return plugins;
		}
	}
}

