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
using System.Linq;
using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.BackendService.ManagementContracts;
using AlarmWorkflow.BackendService.ManagementContracts.Emk;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.UIWidgets.Resources
{
    class ViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets the OperationResourceCollection which should be displayed.
        /// </summary>
        public ObservableCollection<ResourceViewModel> Resources { get; private set; }

        #endregion

        #region Constructors

        internal ViewModel()
        {
            Resources = new ObservableCollection<ResourceViewModel>();
        }

        #endregion

        #region Methods

        internal void OperationChanged(Operation operation)
        {
            try
            {
                ApplyFilteredResources(operation);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(this, ex);

                ApplyAllResourcesFallback(operation);
            }
        }

        private void ApplyFilteredResources(Operation operation)
        {
            Resources.Clear();
            using (var service = ServiceFactory.GetServiceWrapper<IEmkService>())
            {
                IList<EmkResource> emkResources = service.Instance.GetAllResources();

                foreach (OperationResource resource in service.Instance.GetFilteredResources(operation.Resources))
                {
                    EmkResource emk = emkResources.FirstOrDefault(item => item.IsActive && item.IsMatch(resource));

                    Resources.Add(new ResourceViewModel(resource, emk));
                }
            }
        }

        private void ApplyAllResourcesFallback(Operation operation)
        {
            Resources.Clear();
            Resources.AddRange(operation.Resources.Select(item => new ResourceViewModel(item)));
        }

        #endregion
    }
}
