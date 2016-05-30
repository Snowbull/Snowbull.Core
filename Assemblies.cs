/**
 * Plugin Assembly Loader for Snowbull's Plugin API ("Snowbull.API").
 *
 * Copyright 2016 by Lewis Hazell <staticabc@live.co.uk>
 *
 * This file is part of "Snowbull.API".
 * 
 * "Snowbull.API" is free software: you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version
 * 3 of the License, or (at your option) any later version.
 * 
 * "Snowbull.API" is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty 
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See
 * the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with "Snowbull.API". If not, see <http://www.gnu.org/licenses/>.
 *
 * License: GPL-3.0 <https://www.gnu.org/licenses/gpl-3.0.txt>
 */

using System;
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

		internal Type[] Get() {
			Type observer = typeof(Observer.Observer);
			List<Type> types = new List<Type>();
			foreach(Assembly assembly in Loaded)
				foreach(Type type in assembly.GetTypes())
					if(observer.IsAssignableFrom(type))
						types.Add(type);
			return types.ToArray();
		}

	}
}

