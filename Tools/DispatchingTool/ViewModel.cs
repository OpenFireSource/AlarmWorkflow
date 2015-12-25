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
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.BackendService.DispositioningContracts;
using AlarmWorkflow.BackendService.ManagementContracts;
using AlarmWorkflow.BackendService.ManagementContracts.Emk;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.UIContracts;
using AlarmWorkflow.Windows.UIContracts.ViewModels;
using System.Windows;

namespace AlarmWorkflow.Tools.Dispatching
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    class ViewModel : ViewModelBase, IDispositioningServiceCallback, IOperationServiceCallback
    {
        #region Fields

        private WrappedService<IDispositioningService> _dispositioningService;
        private WrappedService<IOperationService> _operationService;
        private static readonly object _lock = new object();
        #endregion

        #region Properties

        /// <summary>
        /// Gets a collection of <see cref="ResourceItem"/>s.
        /// </summary>
        public ObservableCollection<ResourceItem> Resources { get; private set; }

        /// <summary>
        /// Gets or sets the current <see cref="Operation"/>.
        /// </summary>
        public Operation CurrentOperation { get; set; }

        /// <summary>
        /// Gets if an error is currently "available"
        /// </summary>
        public bool Error { get; private set; }

        #endregion

        #region Commands

        #region DispatchCommand

        /// <summary>
        /// The command assigned to the resource buttons.
        /// </summary>
        public ICommand DispatchCommand { get; set; }

        /// <summary>
        /// The command finish the current operation
        /// </summary>
        public ICommand OperationFinishedCommand { get; set; }

        private void OperationFinishedCommand_Execute(object param)
        {
            if (CurrentOperation == null)
            {
                return;
            }

            MessageBoxResult messageBoxResult = MessageBox.Show(Properties.Resources.FinishOperationText, Properties.Resources.FinishOperation, System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                _operationService.Instance.AcknowledgeOperation(CurrentOperation.Id);
            }
        }


        private void DispatchCommand_Execute(object param)
        {
            string id = param as string;
            ResourceItem item = Resources.FirstOrDefault(x => x.EmkResourceItem.Id == id);
            if (item == null)
            {
                return;
            }

            try
            {
                if (_dispositioningService.Instance.GetDispatchedResources(CurrentOperation.Id).Contains(id))
                {
                    _dispositioningService.Instance.Recall(CurrentOperation.Id, id);
                    item.IsDispatched = false;
                }
                else
                {
                    _dispositioningService.Instance.Dispatch(CurrentOperation.Id, id);
                    item.IsDispatched = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(this, ex);
            }
        }

        #endregion

        #endregion

        #region Constructors

        internal ViewModel()
            : base()
        {
            Resources = new ObservableCollection<ResourceItem>();
            Error = false;

            try
            {
                _dispositioningService = ServiceFactory.GetCallbackServiceWrapper<IDispositioningService>(this);
                _operationService = ServiceFactory.GetCallbackServiceWrapper<IOperationService>(this);
            }
            catch (EndpointNotFoundException)
            {
                Error = true;
            }

            if (!Error)
            {
                Task.Factory.StartNew(UpdateCore);
            }

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(Constants.OfpInterval);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        #endregion

        #region Methods

        private void timer_Tick(object sender, EventArgs e)
        {
            Task.Factory.StartNew(Update);
        }

        /// <summary>
        /// Cleans up the service instances when disposing is requested.
        /// </summary>
        protected override void DisposeCore()
        {
            base.DisposeCore();

            if (_dispositioningService != null)
            {
                _dispositioningService.Dispose();
            }
            if (_operationService != null)
            {
                _operationService.Dispose();
            }
        }

        private void Update()
        {
            try
            {
                if (Error)
                {
                    UpdateCore();
                }
                else
                {
                    _operationService.Instance.Ping();
                }
            }
            catch (Exception ex)
            {
                // This exceptions could occur if the service connection was lost.
                if (ex is EndpointNotFoundException || ex is InvalidOperationException || ex is CommunicationException)
                {
                    CurrentOperation = null;
                    Error = true;
                    Application.Current.Dispatcher.Invoke(Resources.Clear);
                }
                else
                {
                    Logger.Instance.LogException(this, ex);
                    throw ex;
                }
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                OnPropertyChanged("Error");
                OnPropertyChanged("Resources");
                OnPropertyChanged("CurrentOperation");
            });
        }

        private void UpdateCore()
        {
            ReconnectIfError();

            Error = false;

            // Get last operation. At the moment dispatching only works for the latest operation.
            IList<int> operationIds = _operationService.Instance.GetOperationIds(Constants.OfpMaxAge, Constants.OfpOnlyNonAcknowledged, 1);
            if (operationIds.Count == 0)
            {
                Application.Current.Dispatcher.Invoke(Resources.Clear);
                CurrentOperation = null;
                OnPropertyChanged("CurrentOperation");
                return;
            }

            int operationId = operationIds.FirstOrDefault();

            bool isNewOperation = (CurrentOperation == null || CurrentOperation.Id != operationId);
            if (!isNewOperation)
            {
                return;
            }
            CurrentOperation = _operationService.Instance.GetOperationById(operationId);

            HandleOperation();
        }

        private void HandleOperation()
        {
            lock (_lock)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Resources.Clear();
                    using (var service = ServiceFactory.GetServiceWrapper<IEmkService>())
                    {
                        List<EmkResource> emkResources = service.Instance.GetAllResources().Where(x => x.IsActive).ToList();
                        IList<OperationResource> alarmedResources = service.Instance.GetFilteredResources(CurrentOperation.Resources);
                        foreach (EmkResource emkResource in emkResources)
                        {
                            ResourceItem resourceItem = new ResourceItem(emkResource) {IsManualDispatchAllowed = !alarmedResources.Any(x => emkResource.IsMatch(x))};

                            Resources.Add(resourceItem);
                        }
                    }

                    string[] dispatchedResources = _dispositioningService.Instance.GetDispatchedResources(CurrentOperation.Id);
                    foreach (ResourceItem item in Resources)
                    {
                        if (dispatchedResources.Contains(item.EmkResourceItem.Id))
                        {
                            item.IsDispatched = true;
                        }
                    }
                });
            }
        }

        private void ReconnectIfError()
        {
            if (Error)
            {
                if (_dispositioningService != null)
                {
                    _dispositioningService.Dispose();
                }
                _dispositioningService = ServiceFactory.GetCallbackServiceWrapper<IDispositioningService>(this);

                if (_operationService != null)
                {
                    _operationService.Dispose();
                }
                _operationService = ServiceFactory.GetCallbackServiceWrapper<IOperationService>(this);
            }
        }

        #endregion

        #region IDispositioningServiceCallback Members

        /// <summary>
        /// Called by the service after a resource was dispatched or recalled.
        /// </summary>
        /// <param name="evt">The event data that describes the event.</param>
        public void OnEvent(DispositionEventArgs evt)
        {
            if (CurrentOperation == null || evt.OperationId != CurrentOperation.Id)
            {
                return;
            }

            if (evt.Action == DispositionEventArgs.ActionType.Dispatch)
            {
                ResourceItem resource = Resources.FirstOrDefault(x => x.EmkResourceItem.Id == evt.EmkResourceId);
                if (resource != null && !resource.IsDispatched)
                {
                    resource.IsDispatched = true;
                    OnPropertyChanged("Resources");
                }
            }
            else if (evt.Action == DispositionEventArgs.ActionType.Recall)
            {
                ResourceItem resource = Resources.FirstOrDefault(x => x.EmkResourceItem.Id == evt.EmkResourceId);
                if (resource != null && resource.IsDispatched)
                {
                    resource.IsDispatched = false;
                    OnPropertyChanged("Resources");
                }
            }
        }

        #endregion

        #region IOperationServiceCallback Members

        /// <summary>
        /// Called when an operation was acknowledged.
        /// </summary>
        /// <param name="id">The id of the operation that was acknowledged.</param>
        public void OnOperationAcknowledged(int id)
        {
            if (CurrentOperation == null || CurrentOperation.Id != id)
            {
                return;
            }

            CurrentOperation = null;

            Application.Current.Dispatcher.Invoke(() =>
            {
                Resources.Clear();
                UpdateCore();
                OnPropertyChanged("CurrentOperation");
            });
        }

        void IOperationServiceCallback.OnNewOperation(Operation op)
        {
            CurrentOperation = op;
            HandleOperation();

            OnPropertyChanged("CurrentOperation");
            OnPropertyChanged("Resources");
        }

        #endregion
    }
}
