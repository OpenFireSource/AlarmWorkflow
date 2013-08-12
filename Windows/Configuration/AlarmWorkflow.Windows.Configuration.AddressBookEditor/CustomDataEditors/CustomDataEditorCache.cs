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
using System.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.Configuration.AddressBookEditor.Extensibility;

namespace AlarmWorkflow.Windows.Configuration.AddressBookEditor.CustomDataEditors
{
    /// <summary>
    /// Serves as a cache for all discovered custom data editors.
    /// </summary>
    public static class CustomDataEditorCache
    {
        #region Fields

        private static readonly List<Type> TypeEditors;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the display names of all type editors as strings for the use within the UI.
        /// </summary>
        public static IEnumerable<string> DisplayNames
        {
            get
            {
                foreach (Type type in TypeEditors)
                {
                    yield return InformationAttribute.GetDisplayName(type);
                }
            }
        }

        #endregion

        #region Constructors

        static CustomDataEditorCache()
        {
            TypeEditors = new List<Type>();

            foreach (var export in ExportedTypeLibrary.GetExports(typeof(ICustomDataEditor)))
            {
                TypeEditors.Add(export.Type);
            }
        }

        #endregion

        #region Methods

        internal static ICustomDataEditor CreateTypeEditorFromDisplayName(string displayName)
        {
            Type type = TypeEditors.FirstOrDefault(t => InformationAttribute.GetDisplayName(t) == displayName);
            if (type != null)
            {
                return (ICustomDataEditor)Activator.CreateInstance(type);
            }
            return null;
        }

        internal static ICustomDataEditor CreateTypeEditor(string identifier)
        {
            // TODO: Using the "Tag" property is ok for now but should be changed (maybe own Attribute for CustomDataEditors?).
            Type type = TypeEditors.FirstOrDefault(t => GetTypeEditorIdentifier(t) == identifier);
            if (type != null)
            {
                return (ICustomDataEditor)Activator.CreateInstance(type);
            }
            return null;
        }

        internal static string GetTypeEditorIdentifier(Type type)
        {
            object tagRaw = InformationAttribute.GetTag(type);
            if (tagRaw == null)
            {
                return null;
            }
            return (string)tagRaw;
        }

        #endregion
    }
}