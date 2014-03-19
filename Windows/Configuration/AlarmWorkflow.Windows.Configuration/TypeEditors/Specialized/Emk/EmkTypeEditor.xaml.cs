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
using System.Windows;
using AlarmWorkflow.BackendService.ManagementContracts.Emk;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ConfigurationContracts;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors.Specialized.Emk
{
    /// <summary>
    /// Interaction logic for EmkTypeEditor.xaml
    /// </summary>
    [Export("EmkTypeEditor", typeof(ITypeEditor))]
    public partial class EmkTypeEditor : ITypeEditor
    {
        #region Fields

        private EmkViewModel _viewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EmkTypeEditor"/> class.
        /// </summary>
        public EmkTypeEditor()
        {
            InitializeComponent();

            _viewModel = new EmkViewModel();
            this.DataContext = _viewModel;
        }

        #endregion

        #region ITypeEditor Members

        object ITypeEditor.Value
        {
            get { return ((ICloneable)_viewModel.Resources).Clone(); }
            set
            {
                EmkResourceCollection instance = value as EmkResourceCollection;
                if (instance == null)
                {
                    instance = new EmkResourceCollection();
                }

                _viewModel.Resources = instance;
            }
        }

        UIElement ITypeEditor.Visual
        {
            get { return this; }
        }

        void ITypeEditor.Initialize(string editorParameter)
        {
        }

        #endregion
    }

    class EmkViewModel : ViewModelBase
    {
        #region Fields

        private EmkResourceCollection _resources;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the instance to edit.
        /// </summary>
        public EmkResourceCollection Resources
        {
            get { return _resources; }
            set
            {
                _resources = value;
                OnPropertyChanged("Resources");
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EmkViewModel"/> class.
        /// </summary>
        public EmkViewModel()
            : base()
        {
            Resources = new EmkResourceCollection();
        }

        #endregion
    }
}
