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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Specialized;
using AlarmWorkflow.Windows.ConfigurationContracts;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    /// <summary>
    /// Interaction logic for ReplaceDictionaryTypeEditor.xaml
    /// </summary>
    [Export("ReplaceDictionaryTypeEditor", typeof(ITypeEditor))]
    public partial class ReplaceDictionaryTypeEditor : UserControl, ITypeEditor, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Gets the ReplaceDictionary-instance for editing.
        /// </summary>
        public ReplaceDictionaryEditWrapper ReplaceDictionary { get; private set; }

        /// <summary>
        /// Gets/sets the currently selected item.
        /// </summary>
        public FakeKeyValuePair Selected { get; set; }

        #endregion

        #region Commands

        #region Command "InsertRowCommand"

        /// <summary>
        /// The InsertRowCommand command.
        /// </summary>
        public ICommand InsertRowCommand { get; private set; }

        private bool InsertRowCommand_CanExecute(object parameter)
        {
            return Selected != null;
        }

        private void InsertRowCommand_Execute(object parameter)
        {
            int indexOf = ReplaceDictionary.IndexOf(Selected);
            ReplaceDictionary.Insert(indexOf, new FakeKeyValuePair());
            OnPropertyChanged("ReplaceDictionary");
            CollectionViewSource.GetDefaultView(ReplaceDictionary).Refresh();
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplaceDictionaryTypeEditor"/> class.
        /// </summary>
        public ReplaceDictionaryTypeEditor()
        {
            InitializeComponent();
            this.DataContext = this;

            CommandHelper.WireupRelayCommands(this);
        }

        #endregion

        #region Methods

        private object GetValue()
        {
            // Look for custom sorting and apply it to the dictionary.
            ICollectionView view = CollectionViewSource.GetDefaultView(this.ReplaceDictionary);
            if (view.SortDescriptions.Count == 1)
            {
                SortDescription sort = view.SortDescriptions[0];

                switch (sort.PropertyName)
                {
                    case "Key": ReplaceDictionary.Sort(f => f.Key, sort.Direction); break;
                    case "Value": ReplaceDictionary.Sort(f => f.Value, sort.Direction); break;
                }
            }

            ReplaceDictionary dict = ReplaceDictionary.GetReplaceDictionary();
            return dict;
        }

        #endregion

        #region ITypeEditor Members

        /// <summary>
        /// Gets/sets the value that is edited.
        /// </summary>
        public object Value
        {
            get { return GetValue(); }
            set
            {
                this.ReplaceDictionary = new ReplaceDictionaryEditWrapper((ReplaceDictionary)value);
                OnPropertyChanged("ReplaceDictionary");
            }
        }

        /// <summary>
        /// Gets the visual element that is editing the value.
        /// </summary>
        public UIElement Visual
        {
            get { return this; }
        }

        void ITypeEditor.Initialize(string editorParameter)
        {
        }

        #endregion

        #region Nested types

        /// <summary>
        /// (Edit wrapper for ReplaceDictionary since DataGrid cannot edit Dictionaries)
        /// </summary>
        public class ReplaceDictionaryEditWrapper : Collection<FakeKeyValuePair>
        {
            #region Properties

            /// <summary>
            /// Gets/sets whether or not to interpret the tokens as regular expressions.
            /// </summary>
            public bool InterpretAsRegex { get; set; }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ReplaceDictionaryEditWrapper"/> class.
            /// </summary>
            /// <param name="source">The source.</param>
            public ReplaceDictionaryEditWrapper(ReplaceDictionary source)
                : base()
            {
                this.InterpretAsRegex = source.InterpretAsRegex;
                foreach (var pair in source.Pairs)
                {
                    this.Add(new FakeKeyValuePair() { Key = pair.Key, Value = pair.Value });
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Returns a new instance of <see cref="ReplaceDictionary"/> with the contents from this instance.
            /// </summary>
            /// <returns></returns>
            public ReplaceDictionary GetReplaceDictionary()
            {
                ReplaceDictionary dict = new ReplaceDictionary();
                dict.InterpretAsRegex = this.InterpretAsRegex;

                foreach (var fakePair in this)
                {
                    // Skip over null or already existing pairs
                    if (fakePair.Key == null || dict.Pairs.ContainsKey(fakePair.Key))// ||
                    //fakePair.Value == null)
                    {
                        continue;
                    }
                    dict.Pairs.Add(fakePair.Key, fakePair.Value);
                }

                return dict;
            }

            /// <summary>
            /// Performs a manual sort using the given sorting function and the sorting direction.
            /// </summary>
            /// <param name="keySelector">User-defined sorting function.</param>
            /// <param name="direction">The sorting direction.</param>
            internal void Sort(Func<FakeKeyValuePair, string> keySelector, ListSortDirection direction)
            {
                IOrderedEnumerable<FakeKeyValuePair> ordered = null;
                if (direction == ListSortDirection.Ascending)
                {
                    ordered = this.Items.OrderBy(keySelector);
                }
                else
                {
                    ordered = this.Items.OrderByDescending(keySelector);
                }

                List<FakeKeyValuePair> temp = ordered.ToList();

                this.Items.Clear();
                this.Items.AddRange(temp);
            }

            #endregion
        }

        /// <summary>
        /// Simple Key-Value-pair-reference-type to enable editing in WPF's DataGrid (the structure "KeyValuePair" is not allowed for editing).
        /// </summary>
        public class FakeKeyValuePair
        {
            /// <summary>
            /// Key
            /// </summary>
            public string Key { get; set; }
            /// <summary>
            /// Value
            /// </summary>
            public string Value { get; set; }
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when the property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var copy = PropertyChanged;
            if (copy != null)
            {
                copy(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}