using System.Collections.Generic;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Job.ComponentNotificator
{
    /// <summary>
    /// Implements an <see cref="IJob"/> to notify custom plugins when a new operation has arrived.
    /// </summary>
    class ComponentNotificatorJob : IJob
    {
        #region Fields

        private List<INotifyable> _observers;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentNotificatorJob"/> class.
        /// </summary>
        public ComponentNotificatorJob()
        {
            // Collect all observers
            _observers = ExportedTypeLibrary.ImportAll<INotifyable>();
            _observers.ForEach(o => o.Initialize());

            // TODO: Since they expose IDisposable, we should give them a chance to dispose (maybe extend IJob?)!
        }

        #endregion

        #region IJob Members

        string IJob.ErrorMessage
        {
            get { return ""; }
        }

        void IJob.Initialize()
        {

        }

        bool IJob.DoJob(Operation operation)
        {
            _observers.ForEach(o => o.Notify(operation));
            return true;
        }

        #endregion
    }
}
