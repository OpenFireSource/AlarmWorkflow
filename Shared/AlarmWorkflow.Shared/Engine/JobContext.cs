using System.Collections.Generic;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Shared.Engine
{
    internal class JobContext : IJobContext
    {
        #region Methods

        internal static IJobContext FromEventArgs(object sender, AlarmSourceEventArgs args)
        {
            IAlarmSource source = sender as IAlarmSource;

            Assertions.AssertNotNull(sender, "sender");

            return new JobContext()
            {
                AlarmSourceName = source.GetType().Name,
                Parameters = args.Parameters,
            };
        }

        #endregion

        #region IContext Members

        /// <summary>
        /// Gets the name of the alarm source that this context runs in.
        /// </summary>
        public string AlarmSourceName { get; private set; }

        /// <summary>
        /// Gets an alarm-source specific array of parameters that are associated with this context and alarm source.
        /// This property is filled by the alarm source.
        /// </summary>
        public IDictionary<string, object> Parameters { get; private set; }

        #endregion
    }
}
