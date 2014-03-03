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
    /// Interaction logic for ExportConfigurationTypeEditor.xaml
    /// </summary>
    [Export("ExportConfigurationTypeEditor", typeof(ITypeEditor))]
    public partial class ExportConfigurationTypeEditor : UserControl, ITypeEditor
    {
        #region Fields

        private ViewModel _viewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportConfigurationTypeEditor"/> class.
        /// </summary>
        public ExportConfigurationTypeEditor()
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
                    _viewModel.ExportedType = type;
                    return;
                }
            }

            throw new InvalidOperationException(string.Format(Properties.Resources.ExportEditorsTypeRequired, editorParameter));
        }

        #endregion

        #region Nested types

        class ViewModel : ViewModelBase
        {
            #region Properties

            /// <summary>
            /// Gets the list of all exports.
            /// </summary>
            public IList<ExportEntryViewModel> Exports { get; private set; }

            /// <summary>
            /// Gets/sets the exported type.
            /// </summary>
            public Type ExportedType { get; set; }

            /// <summary>
            /// Gets/sets the value that is edited.
            /// </summary>
            public object Value
            {
                get
                {
                    ExportConfiguration configuration = new ExportConfiguration();
                    foreach (ExportEntryViewModel entry in Exports)
                    {
                        configuration.Exports.Add(new ExportConfiguration.ExportEntry()
                        {
                            Name = entry.Name,
                            IsEnabled = entry.IsEnabled
                        });
                    }
                    return configuration;
                }
                set
                {
                    ExportConfiguration configuration = (ExportConfiguration)value;

                    List<ExportedType> validExports = ExportedTypeLibrary.GetExports(ExportedType);

                    var newExports = validExports.Select(e => e.Attribute.Alias).Except(configuration.Exports.Select(e => e.Name));
                    var obsoleteExports = configuration.Exports.Select(e => e.Name).Except(validExports.Select(e => e.Attribute.Alias));

                    configuration.Exports.RemoveAll(e => obsoleteExports.Contains(e.Name));
                    configuration.Exports.AddRange(newExports.Select(e => new ExportConfiguration.ExportEntry() { Name = e }));

                    this.Exports = configuration.Exports
                        .Select(e => GetEntryViewModel(e, validExports))
                        .OrderBy(e => e.DisplayName)
                        .ToList();

                    OnPropertyChanged("Exports");
                }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ViewModel"/> class.
            /// </summary>
            public ViewModel()
                : base()
            {

            }

            #endregion

            #region Methods

            private static ExportEntryViewModel GetEntryViewModel(ExportConfiguration.ExportEntry entry, IList<ExportedType> validExports)
            {
                Type myType = validExports.Single(e => e.Attribute.Alias == entry.Name).Type;

                ExportEntryViewModel vm = new ExportEntryViewModel();
                vm.IsEnabled = entry.IsEnabled;
                vm.Name = entry.Name;
                vm.DisplayName = InformationAttribute.GetDisplayName(myType);
                vm.Description = InformationAttribute.GetDescription(myType);
                if (string.IsNullOrWhiteSpace(vm.Description))
                {
                    // If there is no useful description, set it to null to make TargetNullValue in binding effective.
                    vm.Description = null;
                }

                return vm;
            }

            #endregion
        }

        class ExportEntryViewModel : ViewModelBase
        {
            public bool IsEnabled { get; set; }
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string Description { get; set; }
        }

        #endregion
    }
}