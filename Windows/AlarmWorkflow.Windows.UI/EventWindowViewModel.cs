using System.Collections.ObjectModel;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Windows.UI
{
    class EventWindowViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets a list of all available events.
        /// </summary>
        public ObservableCollection<Operation> AvailableEvents { get; private set; }
        /// <summary>
        /// Gets whether or not there are multiple events present. That is, more than one in our case.
        /// </summary>
        public bool AreMultipleEventsPresent
        {
            get { return AvailableEvents.Count > 1; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EventWindowViewModel"/> class.
        /// </summary>
        public EventWindowViewModel()
        {
            AvailableEvents = new ObservableCollection<Operation>();
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

            AvailableEvents.Add(operation);
            OnPropertyChanged("AreMultipleEventsPresent");
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
