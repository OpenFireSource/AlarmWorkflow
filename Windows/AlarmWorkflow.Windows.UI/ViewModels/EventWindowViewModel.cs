using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.Service.WcfServices;
using AlarmWorkflow.Windows.UI.Extensibility;

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
                
                _operationViewer.OnOperationChanged(_selectedEvent.Operation);
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

        #region Commands

        #region Command "AcknowledgeOperationCommand"

        /// <summary>
        /// The AcknowledgeOperationCommand command.
        /// </summary>
        public ICommand AcknowledgeOperationCommand { get; private set; }

        private bool AcknowledgeOperationCommand_CanExecute(object parameter)
        {
            return SelectedEvent != null && !SelectedEvent.Operation.IsAcknowledged;
        }

        private void AcknowledgeOperationCommand_Execute(object parameter)
        {
            // Sanity-checks
            if (SelectedEvent == null || SelectedEvent.Operation.IsAcknowledged)
            {
                return;
            }

            try
            {
                using (var service = ServiceFactory.GetServiceWrapper<IAlarmWorkflowService>())
                {
                    service.Instance.AcknowledgeOperation(SelectedEvent.Operation.Id.ToString());
                    // If we get here, acknowledging was successful --> update operation
                    SelectedEvent.Operation.IsAcknowledged = true;

                    OnPropertyChanged("SelectedEvent");
                    OnPropertyChanged("SelectedEvent.Operation");
                    OnPropertyChanged("SelectedEvent.Operation.IsAcknowledged");

                    Logger.Instance.LogFormat(LogType.Info, this, "Operation with Id '{0}' was acknowledged.", SelectedEvent.Operation.Id);
                }
            }
            catch (Exception ex)
            {
                // Safety first (defensive coding) - don't throw anything here. Instead leave it as it is!
                Logger.Instance.LogFormat(LogType.Error, this, "Could not set operation to 'acknowledged'. Most likely a connection issue or internal error.");
                Logger.Instance.LogException(this, ex);
            }
        }

        #endregion

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
            if (string.IsNullOrWhiteSpace(operationViewerAlias))
            {
                operationViewerAlias = "DefaultOperationView";
            }

            _operationViewer = ExportedTypeLibrary.Import<IOperationViewer>(operationViewerAlias);
            _controlTemplate = new Lazy<FrameworkElement>(() =>
            {
                return _operationViewer.Create();
            });
        }

        /// <summary>
        /// Pushes a new event to the window, either causing it to spawn, or to extend its list box by this event if already shown.
        /// </summary>
        /// <param name="operation">The event to push.</param>
        public void PushEvent(Operation operation)
        {
            // Sanity-check
            if (AvailableEvents.Any(o => o.Operation.Id == operation.Id))
            {
                return;
            }

            // Add the operation and perform a "sanity-sort" (don't trust the web service or whoever...)
            OperationViewModel ovm = new OperationViewModel(operation);
            AvailableEvents.Add(ovm);
            AvailableEvents = new List<OperationViewModel>(AvailableEvents.OrderByDescending(o => o.Operation.Timestamp));

            OnPropertyChanged("AvailableEvents");
            OnPropertyChanged("AreMultipleEventsPresent");

            // If no event is selected yet, select the newest one
            if (SelectedEvent == null)
            {
                SelectedEvent = AvailableEvents[0];
                OnPropertyChanged("SelectedEvent");
            }
        }

        /// <summary>
        /// Clears all events.
        /// </summary>
        public void ClearEvents()
        {
            AvailableEvents.Clear();
            OnPropertyChanged("AreMultipleEventsPresent");
        }

        #endregion

    }
}
