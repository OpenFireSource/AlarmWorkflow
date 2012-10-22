using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.UI.Extensibility;
using AlarmWorkflow.Windows.UI.Models;
using AlarmWorkflow.Windows.UI.Security;

// TODO: The whole, oh-so-modular design (using FrameworkTemplate and Control="{Binding Template}" in XAML) is not the best WPF - change this!

namespace AlarmWorkflow.Windows.UI.ViewModels
{
    class EventWindowViewModel : ViewModelBase
    {
        #region Fields

        private IOperationViewer _operationViewer;
        private Lazy<FrameworkElement> _controlTemplate;

        private OperationViewModel _selectedEvent;

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
        /// Gets the control that is to be displayed in the 
        /// </summary>
        public FrameworkElement Template
        {
            get { return _controlTemplate.Value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EventWindowViewModel"/> class.
        /// </summary>
        public EventWindowViewModel()
        {
            AvailableEvents = new List<OperationViewModel>();

            InitializeOperationViewer();
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
                Logger.Instance.LogFormat(LogType.Warning, this, "Could not find operation viewer with alias '{0}'. Using the default one. Please check the configuration file!", operationViewerAlias);
                _operationViewer = new Views.DefaultOperationView();
            }

            _controlTemplate = new Lazy<FrameworkElement>(() =>
            {
                return _operationViewer.Visual;
            });
        }

        /// <summary>
        /// Pushes a new event to the window, either causing it to spawn, or to extend its list box by this event if already shown.
        /// </summary>
        /// <param name="operation">The event to push.</param>
        /// <returns>Whether or not the event was pushed. This is true if the event was a new one, and false if the event did already exist.</returns>
        public bool PushEvent(Operation operation)
        {
            // Sanity-check
            if (AvailableEvents.Any(o => o.Operation.Id == operation.Id))
            {
                return false;
            }

            // Notify operation viewer of this new operation (only if the operation is not acknowledged and thus new)
            if (!operation.IsAcknowledged)
            {
                _operationViewer.OnNewOperation(operation);
            }

            // Add the operation and perform a "sanity-sort" (don't trust the web service or whoever...)
            OperationViewModel ovm = new OperationViewModel(operation);
            AvailableEvents.Add(ovm);
            AvailableEvents = new List<OperationViewModel>(AvailableEvents.OrderByDescending(o => o.Operation.Timestamp));

            OnPropertyChanged("AvailableEvents");
            OnPropertyChanged("AreMultipleEventsPresent");

            // If no event is selected yet, select the newest one (also do this if the selected operation is older. Newer operations have priority!).
            if (SelectedEvent == null || (SelectedEvent != null && SelectedEvent.Operation.Timestamp < AvailableEvents[0].Operation.Timestamp))
            {
                SelectedEvent = AvailableEvents[0];
            }

            // Call the UI-jobs now on this specific job
            App.GetApp().ExtensionManager.RunUIJobs(_operationViewer, operation);

            // When the jobs are done, change over to the job (necessary in most cases albeit not perfect solution :-/ )
            _operationViewer.OnOperationChanged(SelectedEvent.Operation);

            return true;
        }

        private void RemoveEvent(OperationViewModel operation)
        {
            AvailableEvents.Remove(operation);
            AvailableEvents = new List<OperationViewModel>(AvailableEvents.OrderByDescending(o => o.Operation.Timestamp));

            OnPropertyChanged("AvailableEvents");
            OnPropertyChanged("AreMultipleEventsPresent");
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

            try
            {
                using (var service = InternalServiceProxy.GetServiceInstance())
                {
                    service.Instance.AcknowledgeOperation(SelectedEvent.Operation.Id);
                    // If we get here, acknowledging was successful --> update operation
                    SelectedEvent.Operation.IsAcknowledged = true;

                    Logger.Instance.LogFormat(LogType.Info, this, "Operation with Id '{0}' was acknowledged.", SelectedEvent.Operation.Id);
                }

                // If we shall go to the next operation afterwards
                if (gotoNextOperation)
                {
                    RemoveEvent(SelectedEvent);
                    SelectedEvent = AvailableEvents.FirstOrDefault();
                }

                OnPropertyChanged("SelectedEvent");
                OnPropertyChanged("SelectedEvent.Operation");
                OnPropertyChanged("SelectedEvent.Operation.IsAcknowledged");
            }
            catch (Exception ex)
            {
                // Safety first (defensive coding) - don't throw anything here. Instead leave it as it is!
                Logger.Instance.LogFormat(LogType.Error, this, "Could not set operation to 'acknowledged'. Most likely a connection issue or internal error.");
                Logger.Instance.LogException(this, ex);
            }
        }

        #endregion

    }
}
