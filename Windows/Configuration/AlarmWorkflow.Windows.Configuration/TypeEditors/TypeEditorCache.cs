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
