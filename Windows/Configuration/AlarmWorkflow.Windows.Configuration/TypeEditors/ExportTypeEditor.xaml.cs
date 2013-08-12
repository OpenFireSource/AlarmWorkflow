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
using System.Windows.Controls;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ConfigurationContracts;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    /// <summary>
    /// Interaction logic for ExportTypeEditor.xaml
    /// </summary>
    [Export("ExportTypeEditor", typeof(ITypeEditor))]
    public partial class ExportTypeEditor : UserControl, ITypeEditor
    {
        #region Fields

        private ViewModel _viewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportTypeEditor"/> class.
        /// </summary>
        public ExportTypeEditor()
        {
            InitializeComponent();

            _viewModel = new ViewModel();
            this.DataContext = _viewModel;
        }

        #endregion

        #region ITypeEditor Members

        object ITypeEditor.Value
        {
            get { return _viewModel.Value; }
            set { _viewModel.Value = value; }
        }

        /// <summary>
        /// Gets the visual element that is editing the value.
        /// </summary>
        public System.Windows.UIElement Visual
        {
            get { return this; }
        }

        void ITypeEditor.Initialize(string editorParameter)
        {
            if (!string.IsNullOrWhiteSpace(editorParameter))
            {
                Type type = Type.GetType(editorParameter);
                if (type != null)
                {
                    var exports = ExportedTypeLibrary.GetExports(type).Select(e => GetEntryViewModel(e)).OrderBy(e => e.DisplayName);
                    _viewModel.Exports.AddRange(exports);
                    return;
                }
            }

            throw new InvalidOperationException(string.Format(Properties.Resources.ExportEditorsTypeRequired, editorParameter));
        }

        #endregion

        #region Methods

        private static ExportEntryViewModel GetEntryViewModel(ExportedType export)
        {
            ExportEntryViewModel vm = new ExportEntryViewModel();
            vm.Name = export.Attribute.Alias;
            vm.DisplayName = InformationAttribute.GetDisplayName(export.Type);
            vm.Description = InformationAttribute.GetDescription(export.Type);
            if (string.IsNullOrWhiteSpace(vm.Description))
            {
                // If there is no useful description, set it to null to make TargetNullValue in binding effective.
                vm.Description = null;
            }

            return vm;
        }

        #endregion

        #region Nested types

        class ViewModel : ViewModelBase
        {
            #region Fields

            private object _rawValue;

            #endregion

            #region Properties

            /// <summary>
            /// Gets/sets the list of exports to use for displaying.
            /// </summary>
            public IList<ExportEntryViewModel> Exports { get; set; }
            /// <summary>
            /// Gets/sets the currently selected item.
            /// </summary>
            public ExportEntryViewModel SelectedItem { get; set; }
            /// <summary>
            /// Gets/sets the object from the settings.
            /// Wraps it automatically for usage.
            /// </summary>
            public object Value
            {
                get
                {
                    // Sometimes the alias of an export is set, which doesn't exist anymore.
                    // In this case return the initially stored name instead.
                    if (SelectedItem == null)
                    {
                        return _rawValue;
                    }
                    return SelectedItem.Name;
                }
                set
                {
                    _rawValue = value;

                    string name = (string)value;
                    SelectedItem = Exports.FirstOrDefault(e => e.Name == name);
                    OnPropertyChanged("SelectedItem");
                }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ViewModel"/> class.
            /// </summary>
            public ViewModel()
            {
                Exports = new List<ExportEntryViewModel>();
            }

            #endregion
        }

        class ExportEntryViewModel : ViewModelBase
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string Description { get; set; }
        }

        #endregion
    }
}