﻿using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Snowbull.API {
	internal class Assemblies {
		private ImmutableArray<Assembly> Loaded {
			get;
			set;
		}

		protected Assemblies(ImmutableArray<Assembly> assemblies) {
			if(assemblies == null) throw new System.ArgumentNullException("assemblies");
			Loaded = assemblies;
		}

		internal static Assemblies Load(string folder) {
			if(folder == null) throw new System.ArgumentNullException("folder");
			return new Assemblies(GetAssemblies(folder).ToImmutableArray());
		}

		private static Assembly[] GetAssemblies(string folder) {
			if(folder == null) throw new System.ArgumentNullException("folder");
			string[] files = System.IO.Directory.GetFiles(folder, "*.dll");
			Assembly[] assemblies = new Assembly[files.Length];
			for(int i = 0; i < files.Length; i++) {
				string file = files[i];
				assemblies[i] = Assembly.LoadFrom(file);
			}
			return assemblies;
		}

		internal Observer.Observer[] Get(IServer server) {
			Type observer = typeof(Observer.Observer);
			List<Observer.Observer> observers = new List<Observer.Observer>();
			foreach(Assembly assembly in Loaded)
				foreach(Type type in assembly.GetTypes())
					if(observer.IsAssignableFrom(type)) observers.Add((Observer.Observer) Activator.CreateInstance(type, new object[] { server }));
			return observers.ToArray();
		}

	}
}

