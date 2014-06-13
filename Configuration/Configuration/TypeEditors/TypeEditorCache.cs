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
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.Configuration.Properties;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    sealed class TypeEditorCache
    {
        private static readonly Dictionary<string, Type> TypeEditors;

        static TypeEditorCache()
        {
            TypeEditors = new Dictionary<string, Type>();
            TypeEditors[""] = typeof(TypeEditors.DefaultTypeEditor);

            foreach (ExportedType export in ExportedTypeLibrary.GetExports(typeof(ITypeEditor)))
            {
                RegisterTypeEditor(export.Attribute.Alias, export.Type);

                foreach (ConfigurationTypeEditorAttribute attribute in export.Type.GetCustomAttributes(typeof(ConfigurationTypeEditorAttribute), false))
                {
                    RegisterTypeEditor(attribute.SourceType.FullName, export.Type);
                }
            }
            // TODO: Better editors!
            // TODO: 'SimpleXmlTextEditor' still needed?
            RegisterTypeEditor("SimpleXmlTextEditor", typeof(TypeEditors.StringArrayTypeEditor));
        }

        private static void RegisterTypeEditor(string key, Type value)
        {
            if (TypeEditors.ContainsKey(key))
            {
                // Don't just overwrite. Log this and go ahead.
                Logger.Instance.LogFormat(LogType.Warning, typeof(TypeEditorCache), Resources.TypeEditorAlreadyRegisteredWarning, key, value.Assembly);
                return;
            }

            TypeEditors[key] = value;
            Logger.Instance.LogFormat(LogType.Trace, typeof(TypeEditorCache), Resources.TypeEditorRegistered, value, key);
        }


        internal static ITypeEditor CreateTypeEditor(string editor)
        {
            if (!TypeEditors.ContainsKey(editor))
            {
                editor = "";
            }

            return (ITypeEditor)Activator.CreateInstance(TypeEditors[editor]);
        }
    }
}