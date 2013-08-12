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
using System.ServiceModel;
using System.Timers;
using System.Windows;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.ServiceContracts;
using AlarmWorkflow.Windows.UI.Models;
using AlarmWorkflow.Windows.UI.Properties;
using AlarmWorkflow.Windows.UIContracts.Extensibility;
using AlarmWorkflow.Windows.UIContracts.Security;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.UI.ViewModels
{
    /// <summary>
    /// Represents the main window's VM.
    /// </summary>
    class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private IOperationViewer _operationViewer;
        private Lazy<FrameworkElement> _busyTemplate;

        private OperationViewModel _selectedEvent;

        private Timer _servicePollingTimer;
        private bool _isMissingServiceConnectionHintVisible;
        private Timer _switchTimer;

        private readonly object TimerLock = new object();

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list of all available events.
        /// </summary>
        public List<OperationViewModel> AvailableEvents { get; private set; }
        /// <summary>
        /// Gets/sets the selected event.
        /// </summary>
        public OperationViewModel SelectedEvent
        {
            get { return _selectedEvent; }
            set
            {
                if (_selectedEvent == value)
                {
                    return;
                }

                _selectedEvent = value;
                OnPropertyChanged("SelectedEvent");

                Operation operationNew = _selectedEvent != null ? _selectedEvent.Operation : null;
                _operationViewer.OnOperationChanged(operationNew);
            }
        }
        /// <summary>
        /// Gets whether or not there are multiple events present. That is, more than one in our case.
        /// </summary>
        public bool AreMultipleEventsPresent
        {
            get { return AvailableEvents.Count > 1; }
        }
        /// <summary>
        /// Gets whether or not there are any events at all. The outcome of this property decides which template to show.
        /// </summary>
        public bool HasDisplayableEvents
        {
            get { return AvailableEvents.Count > 0; }
        }
        /// <summary>
        /// Gets whether or not a connection to the service is established.
        /// </summary>
        public bool IsMissingServiceConnectionHintVisible
        {
            get { return _isMissingServiceConnectionHintVisible; }
            private set
            {
                if (value == _isMissingServiceConnectionHintVisible)
                {
                    return;
                }

                _isMissingServiceConnectionHintVisible = value;
                OnPropertyChanged("IsMissingServiceConnectionHintVisible");
            }
        }

        /// <summary>
        /// Gets the template that is displayed when there are actually alarms to display.
        /// </summary>
        public FrameworkElement BusyTemplate
        {
            get { return _busyTemplate.Value; }
        }
        /// <summary>
        /// Gets the template that is visible when there are no alarms to display.
        /// </summary>
        public FrameworkElement IdleTemplate { get; private set; }

        /// <summary>
        /// Gets/sets the UI-Scale-Factor.
        /// </summary>
        public double UiScaleFactor
        {
            get { return App.GetApp().Configuration.ScaleFactor; }
            set
            {
                App.GetApp().Configuration.ScaleFactor = value;
                OnPropertyChanged("UiScaleFactor");
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            AvailableEvents = new List<OperationViewModel>();

            InitializeOperationViewer();

            _servicePollingTimer = new Timer(Constants.OfpInterval);
            _servicePollingTimer.Elapsed += ServicePollingTimer_Elapsed;
            _servicePollingTimer.Start();

            if (App.GetApp().Configuration.SwitchAlarms)
            {
                _switchTimer = new Timer(App.GetApp().Configuration.SwitchTime * 1000);
                _switchTimer.Elapsed += _switchTimer_Elapsed;
                _switchTimer.Start();
            }
        }

        #endregion

        #region Methods

        private void InitializeOperationViewer()
        {
            string operationViewerAlias = App.GetApp().Configuration.OperationViewer;
            _operationViewer = ExportedTypeLibrary.Import<IOperationViewer>(operationViewerAlias);

            // If there is no operation viewer defined or it could not be found, use the default one and log it
            if (_operationViewer == null)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Resources.DesiredOperationViewerNotFound, operationViewerAlias);
                _operationViewer = new Views.DefaultOperationView();
            }

            _busyTemplate = new Lazy<FrameworkElement>(() =>
            {
                return _operationViewer.Visual;
            });
        }

        private void PushEvent(Operation operation)
        {
            if (ContainsEvent(operation.Id))
            {
                return;
            }

            bool isOperationNew = !operation.IsAcknowledged;
            bool isNewToList = AddOperation(operation);

            // If no event is selected yet, select the newest one (also do this if the selected operation is older. Newer operations have priority!).
            if (SelectedEvent == null || (SelectedEvent != null && SelectedEvent.Operation.TimestampIncome < AvailableEvents[0].Operation.TimestampIncome))
            {
                SelectedEvent = AvailableEvents[0];
            }

            if (isNewToList)
            {
                UpdateProperties();
                if (isOperationNew)
                {
                    _operationViewer.OnNewOperation(operation);
                    App.GetApp().ExtensionManager.RunUIJobs(_operationViewer, operation);
                }
            }

            _operationViewer.OnOperationChanged(SelectedEvent.Operation);
        }

        /// <summary>
        /// Adds the operation to the list of available alarms and returns if the operation is a new one.
        /// An operation is "new" if it was once added to the list.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns>Whether or not the added operation is "new".</returns>
        private bool AddOperation(Operation operation)
        {
            bool operationIsInList = ContainsEvent(operation.Id);

            // Add the operation and perform a "sanity-sort" (don't trust the web service or whoever...)
            OperationViewModel ovm = new OperationViewModel(operation);
            AvailableEvents.Add(ovm);
            AvailableEvents = new List<OperationViewModel>(AvailableEvents.OrderByDescending(o => o.Operation.TimestampIncome));

            // If we shall have a limit of alarms in the UI...
            int maxAlarmsInUI = App.GetApp().Configuration.MaxAlarmsInUI;
            if (maxAlarmsInUI > 0 && (AvailableEvents.Count > maxAlarmsInUI))
            {
                // ... remove older alarms until we have our amount.
                int count = AvailableEvents.Count - maxAlarmsInUI;
                AvailableEvents.RemoveRange(maxAlarmsInUI, count);
            }

            bool operationWasAddedToList = ContainsEvent(operation.Id);
            return (!operationIsInList && operationWasAddedToList);
        }

        private void UpdateProperties()
        {
            OnPropertyChanged("AvailableEvents");
            OnPropertyChanged("AreMultipleEventsPresent");
            OnPropertyChanged("HasDisplayableEvents");
        }

        private void RemoveEvent(OperationViewModel operation)
        {
            AvailableEvents.Remove(operation);
            AvailableEvents = new List<OperationViewModel>(AvailableEvents.OrderByDescending(o => o.Operation.TimestampIncome));

            UpdateProperties();
        }

        /// <summary>
        /// Acknowledges the selected (current) operation.
        /// </summary>
        /// <param name="gotoNextOperation">Whether or not to change to the next operation (recommended).</param>
        public void AcknowledgeCurrentOperation(bool gotoNextOperation)
        {
            // Sanity-checks
            if (SelectedEvent == null || SelectedEvent.Operation.IsAcknowledged)
            {
                return;
            }

            // Require confirmation of this action
            if (!ServiceProvider.Instance.GetService<ICredentialConfirmationDialogService>().Invoke("Einsatz zur Kenntnis nehmen", AuthorizationMode.SimpleConfirmation))
            {
                return;
            }

            AcknowledgeOperationAndGoToFirst(SelectedEvent);
        }

        private void AcknowledgeOperationAndGoToFirst(OperationViewModel operation)
        {
            try
            {
                if (!operation.Operation.IsAcknowledged)
                {
                    using (var service = InternalServiceProxy.GetServiceInstance())
                    {
                        service.Instance.AcknowledgeOperation(operation.Operation.Id);
                    }

                    operation.Operation.IsAcknowledged = true;
                    Logger.Instance.LogFormat(LogType.Info, this, Resources.AcknowledgedOperation, operation.Operation.Id);
                }

                RemoveEvent(operation);

                // Always select the first operation if possible
                SelectedEvent = AvailableEvents.FirstOrDefault();

                OnPropertyChanged("SelectedEvent");
                OnPropertyChanged("SelectedEvent.Operation");
                OnPropertyChanged("SelectedEvent.Operation.IsAcknowledged");
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Resources.AcknowledgeOperationFailed);
                Logger.Instance.LogException(this, ex);
            }
        }

        private bool ContainsEvent(int operationId)
        {
            return AvailableEvents.Any(o => o.Operation.Id == operationId);
        }

        private void NextAlarm()
        {
            if (AvailableEvents.Count > 1)
            {
                int current = AvailableEvents.IndexOf(SelectedEvent);
                if (current == AvailableEvents.Count - 1)
                {
                    SelectedEvent = AvailableEvents[0];
                }
                else
                {
                    SelectedEvent = AvailableEvents[current + 1];
                }

                OnPropertyChanged("SelectedEvent.Operation");
                OnPropertyChanged("SelectedEvent.Operation.IsAcknowledged");
            }
        }

        private void _switchTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (TimerLock)
            {
                App.Current.Dispatcher.Invoke(() => NextAlarm());
            }
        }

        private void ServicePollingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (TimerLock)
            {
                PollServiceAndRetrieveOperations();
                CheckAutomaticAcknowledging();
            }
        }

        private void PollServiceAndRetrieveOperations()
        {
            try
            {
                using (var service = InternalServiceProxy.GetServiceInstance())
                {
                    IList<int> operations = service.Instance.GetOperationIds(Constants.OfpMaxAge, Constants.OfpOnlyNonAcknowledged, Constants.OfpLimitAmount);

                    IsMissingServiceConnectionHintVisible = false;

                    if (operations.Count == 0)
                    {
                        return;
                    }

                    foreach (int operationId in operations)
                    {
                        if (ContainsEvent(operationId))
                        {
                            continue;
                        }

                        OperationItem operationItem = service.Instance.GetOperationById(operationId);
                        Operation operation = operationItem.ToOperation();

                        if (ShouldAutomaticallyAcknowledgeOperation(operation))
                        {
                            service.Instance.AcknowledgeOperation(operation.Id);
                        }
                        else
                        {
                            App.Current.Dispatcher.Invoke(() => PushEvent(operation));
                        }
                    }
                }
            }
            catch (EndpointNotFoundException)
            {
                IsMissingServiceConnectionHintVisible = true;
            }
            catch (Exception ex)
            {
                IsMissingServiceConnectionHintVisible = false;
                Logger.Instance.LogException(this, ex);
            }
        }

        private bool ShouldAutomaticallyAcknowledgeOperation(Operation operation)
        {
            if (!App.GetApp().Configuration.AutomaticOperationAcknowledgement.IsEnabled)
            {
                return false;
            }

            int daage = App.GetApp().Configuration.AutomaticOperationAcknowledgement.MaxAge;
            TimeSpan dat = daage > 0 ? TimeSpan.FromMinutes(daage) : Operation.DefaultAcknowledgingTimespan;
            return !operation.IsAcknowledged && (DateTime.Now - operation.TimestampIncome) > dat;
        }

        private void CheckAutomaticAcknowledging()
        {
            foreach (OperationViewModel item in AvailableEvents.ToList().OrderBy(o => o.Operation.TimestampIncome))
            {
                if (!ShouldAutomaticallyAcknowledgeOperation(item.Operation))
                {
                    continue;
                }

                App.Current.Dispatcher.Invoke(() => AcknowledgeOperationAndGoToFirst(item));
            }
        }

        #endregion
    }
}