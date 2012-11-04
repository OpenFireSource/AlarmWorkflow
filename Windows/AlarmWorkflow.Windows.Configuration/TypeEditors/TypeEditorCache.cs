using System;
using System.Linq;
using System.Collections.Generic;
using AlarmWorkflow.Shared.Core;
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

            foreach (var export in ExportedTypeLibrary.GetExports(typeof(ITypeEditor)))
            {
                // 1. Use alias.
                TypeEditors[export.Attribute.Alias] = export.Type;

                // 2. Use attribute if available.
                ConfigurationTypeEditorAttribute attribute = (ConfigurationTypeEditorAttribute)export.Type.GetCustomAttributes(typeof(ConfigurationTypeEditorAttribute), false).FirstOrDefault();
                if (attribute != null)
                {
                    TypeEditors[attribute.SourceType.FullName] = export.Type;
                }
            }
            // TODO: Better editors!
            TypeEditors["SimpleXmlTextEditor"] = typeof(TypeEditors.StringArrayTypeEditor);
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
