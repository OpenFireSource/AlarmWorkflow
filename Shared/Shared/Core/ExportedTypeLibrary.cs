// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Provides access to types exported with the <see cref="T:ExportAttribute"/>.
    /// </summary>
    public static class ExportedTypeLibrary
    {
        #region Fields

        private static IList<ExportedType> _exports;

        #endregion

        #region Constructors

        static ExportedTypeLibrary()
        {
            _exports = new List<ExportedType>(16);

            Initialize();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the exported types library.
        /// </summary>
        private static void Initialize()
        {
            // if there are no desired assemblies then we take all assemblies we can find in the working directory
            List<string> assembliesToScan = new List<string>();
            string directoryToScan = Utilities.GetWorkingDirectory(Assembly.GetExecutingAssembly());
            // alright, lets scan all assemblies in the working directory
            assembliesToScan.AddRange(Directory.GetFiles(directoryToScan, "*.exe", SearchOption.TopDirectoryOnly));
            assembliesToScan.AddRange(Directory.GetFiles(directoryToScan, "*.dll", SearchOption.TopDirectoryOnly));

            // load and check each assembly's types
            foreach (string file in assembliesToScan)
            {
                try
                {
                    Assembly assembly = Assembly.Load(AssemblyName.GetAssemblyName(file));

                    ScanAssembly(assembly);
                }
                catch (Exception)
                {
                    // An exception can occur if we scan an unmanaged dll, or any dll that may contain errors but we ignore them
                }
            }
        }

        private static void ScanAssembly(Assembly assembly)
        {
            int amount = 0;

            // process all types (even private ones)
            Type[] types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                Type t = types[i];

                var exports = t.GetCustomAttributes(typeof(ExportAttribute), false);
                if (exports.Length == 1)
                {
                    ExportAttribute export = exports[0] as ExportAttribute;
                    _exports.Add(new ExportedType(export, t));
                }

                amount++;
            }
        }

        private static bool ImplementsInterface(Type type, Type interfaceType)
        {
            Type[] interfaces = type.GetInterfaces();
            for (int j = 0; j < interfaces.Length; j++)
            {
                Type iface = interfaces[j];

                if (iface == interfaceType)
                {
                    return true;
                }
            }
            return false;
        }

        private static ExportedType GetExport(string alias, Type interfaceType)
        {
            return GetExports(interfaceType).Find(e => e.Attribute.Alias.Equals(alias));
        }

        /// <summary>
        /// Searches the first exported occurrence of the given type and creates an instance of it.
        /// </summary>
        /// <typeparam name="T">The type of the exported interface to get the first export of.</typeparam>
        /// <returns></returns>
        public static T Import<T>()
        {
            var exports = GetExports(typeof(T));
            if (exports.Count > 0)
            {
                return (T)Activator.CreateInstance(exports[0].Type);
            }

            return default(T);
        }

        /// <summary>
        /// Imports the exported type with the given alias and type and creates an instance of it.
        /// </summary>
        /// <typeparam name="T">The type of the exported interface to get the export of.</typeparam>
        /// <param name="alias">The alias of the export to get.</param>
        /// <returns></returns>
        public static T Import<T>(string alias)
        {
            ExportedType export = GetExport(alias, typeof(T));
            if (export.Type != null)
            {
                return (T)Activator.CreateInstance(export.Type);
            }

            return default(T);
        }

        /// <summary>
        /// Imports all selected exports and creates instances out of them.
        /// </summary>
        /// <typeparam name="T">The type of the exported interface to get the export of.</typeparam>
        /// <returns></returns>
        public static List<T> ImportAll<T>()
        {
            List<T> list = new List<T>();
            foreach (var item in GetExports(typeof(T)))
            {
                list.Add(item.CreateInstance<T>());
            }
            return list;
        }

        /// <summary>
        /// Returns a list containing all types that export the desired interface.
        /// </summary>
        /// <param name="interfaceType">The interface type to get all exports for.</param>
        /// <returns></returns>
        public static List<ExportedType> GetExports(Type interfaceType)
        {
            List<ExportedType> exports = new List<ExportedType>();
            foreach (ExportedType export in _exports)
            {
                if (ImplementsInterface(export.Type, interfaceType))
                {
                    exports.Add(export);
                }
            }
            return exports;
        }

        /// <summary>
        /// Makes an inverse search to find out the alias for a specific type.
        /// </summary>
        /// <param name="type">The type to get the alias for. Must not be null.</param>
        /// <returns>A string representing the alias for the given type.</returns>
        public static string GetExportAlias(Type type)
        {
            Assertions.AssertNotNull(type, "type");

            ExportAttribute[] attributes = (ExportAttribute[])type.GetCustomAttributes(typeof(ExportAttribute), false);
            if (attributes.Length == 1)
            {
                return attributes[0].Alias;
            }
            return null;
        }

        #endregion
    }
}