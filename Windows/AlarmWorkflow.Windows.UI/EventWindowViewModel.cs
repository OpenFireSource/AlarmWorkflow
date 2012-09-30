using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.Service.WcfServices;
using System.Linq;

namespace AlarmWorkflow.Windows.UI
{
    class EventWindowViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets a list of all available events.
        /// </summary>
        public List<Operation> AvailableEvents { get; private set; }
        /// <summary>
        /// Gets/sets the selected event.
        /// </summary>
        public Operation SelectedEvent { get; set; }
        /// <summary>
        /// Gets whether or not there are multiple events present. That is, more than one in our case.
        /// </summary>
        public bool AreMultipleEventsPresent
        {
            get { return AvailableEvents.Count > 1; }
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
            return SelectedEvent != null && !SelectedEvent.IsAcknowledged;
        }

        private void AcknowledgeOperationCommand_Execute(object parameter)
        {
            // Sanity-checks
            if (SelectedEvent == null || SelectedEvent.IsAcknowledged)
            {
                return;
            }

            try
            {
                using (var service = ServiceFactory.GetServiceWrapper<IAlarmWorkflowService>())
                {
                    service.Instance.AcknowledgeOperation(SelectedEvent.Id);
                    // If we get here, acknowledging was successful --> update operation
                    SelectedEvent.IsAcknowledged = true;

                    OnPropertyChanged("SelectedEvent");

                    Logger.Instance.LogFormat(LogType.Info, this, "Operation with Id '{0}' was acknowledged.", SelectedEvent.Id);
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
            AvailableEvents = new List<Operation>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Pushes a new event to the window, either causing it to spawn, or to extend its list box by this event if already shown.
        /// </summary>
        /// <param name="operation">The event to push.</param>
        public void PushEvent(Operation operation)
        {
            // Sanity-check
            if (AvailableEvents.Contains(operation))
            {
                return;
            }

            // Add the operation and perform a "sanity-sort" (don't trust the web service or whoever...)
            AvailableEvents.Add(operation);
            AvailableEvents = new List<Operation>(AvailableEvents.OrderByDescending(o => o.Timestamp));

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
