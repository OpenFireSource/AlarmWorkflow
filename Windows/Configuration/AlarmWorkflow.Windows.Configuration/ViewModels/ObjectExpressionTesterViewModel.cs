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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.ObjectExpressions;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.ViewModels
{
    class ObjectExpressionTesterViewModel : ViewModelBase
    {
        #region Constants

        private const string ObjectTestDatabaseFileName = "ObjectTestDatabase.xml";

        #endregion

        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list containing all seed entries from the database file.
        /// </summary>
        public IList<SeedEntry> Entries { get; private set; }
        /// <summary>
        /// Gets/sets the selected entry.
        /// </summary>
        public SeedEntry SelectedEntry { get; set; }
        /// <summary>
        /// Gets/sets the text the user has entered.
        /// </summary>
        public string InputText { get; set; }
        /// <summary>
        /// Gets the result of the formatting process.
        /// </summary>
        public string ResultText { get; private set; }

        #endregion

        #region Commands

        #region Command "TestUserInputCommand"

        /// <summary>
        /// The TestUserInputCommand command.
        /// </summary>
        public ICommand TestUserInputCommand { get; private set; }

        private bool TestUserInputCommand_CanExecute(object parameter)
        {
            return SelectedEntry != null;
        }

        private void TestUserInputCommand_Execute(object parameter)
        {
            string result = "";
            try
            {
                if (SelectedEntry.UseIFormattable)
                {
                    IFormattable formattable = (IFormattable)SelectedEntry.Instance;
                    result = formattable.ToString(InputText, CultureInfo.CurrentUICulture);
                }
                else
                {
                    ExtendedObjectExpressionFormatter<object> formatter = new ExtendedObjectExpressionFormatter<object>();
                    result = formatter.ToString(SelectedEntry.Instance, InputText);
                }
            }
            catch (InvalidCastException ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.SeedEntryTestTypeIsNotIFormattable, SelectedEntry.Instance.GetType().Name);
                Logger.Instance.LogException(this, ex);

                result = Properties.Resources.SeedEntryTestFailed.ToUpperInvariant();
            }
            catch (AssertionFailedException ex)
            {
                Logger.Instance.LogException(this, ex);

                result = ex.Message.ToUpperInvariant();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(this, ex);

                result = Properties.Resources.SeedEntryTestFailed.ToUpperInvariant();
            }

            if (result != null)
            {
                ResultText = result;
                OnPropertyChanged("ResultText");
            }
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectExpressionTesterViewModel"/> class.
        /// </summary>
        public ObjectExpressionTesterViewModel()
            : base()
        {
            InputText = string.Empty;

            Entries = new List<SeedEntry>(LoadDatabaseFile().OrderBy(e => e.EntryType));
            if (Entries.Count > 0)
            {
                SelectedEntry = Entries[0];
                OnPropertyChanged("SelectedEntry");
            }
        }

        #endregion

        #region Methods

        private IEnumerable<SeedEntry> LoadDatabaseFile()
        {
            string path = Path.Combine(Utilities.GetWorkingDirectory(), ObjectTestDatabaseFileName);
            if (File.Exists(path))
            {
                XDocument doc = XDocument.Load(path);
                if (!doc.IsXmlValid(Properties.Resources.ObjectTestDatabaseSchema))
                {
                    Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.SeedEntryParseError);
                    yield break;
                }

                foreach (XElement elem in doc.Root.Elements("Seed"))
                {
                    SeedEntry entry = ParseElement(elem);
                    if (entry != null)
                    {
                        yield return entry;
                    }
                }
            }
        }

        private SeedEntry ParseElement(XElement elem)
        {
            string typeName = elem.Attribute("Type").Value;

            Type type = Type.GetType(typeName);
            if (type == null)
            {
                return null;
            }

            if (type.GetConstructor(Type.EmptyTypes) == null)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.SeedEntryTypeHasNoDefaultConstructor, type.FullName);
                return null;
            }

            object instance = Activator.CreateInstance(type);

            foreach (XElement prop in elem.Elements("Set"))
            {
                string expression = prop.Attribute("Expression").Value;
                string valueRaw = prop.Value;

                PropertyInfo property = null;
                object target = null;
                if (!ObjectExpressionTools.GetPropertyFromExpression(instance, expression, false, out property, out target))
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.SeedEntryPropertyNotFound, expression, type.Name);
                    break;
                }

                object value = Convert.ChangeType(valueRaw, property.PropertyType);

                if (!ObjectExpressionTools.TrySetValueFromExpression(instance, expression, value))
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, Properties.Resources.SeedEntrySetPropertyFailed, expression, type.Name);
                    break;
                }
            }

            SeedEntry entry = new SeedEntry();
            entry.Name = elem.Attribute("Name").Value;
            entry.UseIFormattable = bool.Parse(elem.Attribute("UseIFormattable").Value);
            entry.EntryType = type;
            entry.Instance = instance;
            return entry;
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents a single seeded entry, defining a concrete instance with custom property values.
        /// </summary>
        public class SeedEntry
        {
            /// <summary>
            /// Gets/sets a visual name for this entry.
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Gets/sets the defined type for this entry.
            /// </summary>
            public Type EntryType { get; set; }
            /// <summary>
            /// Gets/sets the actual instance.
            /// </summary>
            public object Instance { get; set; }
            /// <summary>
            /// Gets/sets whether or not to use the type's ToString() method instead of the formatter.
            /// This is especially useful for types that do custom property resolving.
            /// </summary>
            public bool UseIFormattable { get; set; }
        }

        #endregion
    }
}
