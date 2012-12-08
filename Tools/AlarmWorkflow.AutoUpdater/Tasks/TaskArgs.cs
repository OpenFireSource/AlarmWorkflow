using System;
using System.Collections.Generic;

namespace AlarmWorkflow.Tools.AutoUpdater.Tasks
{
    class TaskArgs
    {
        internal TaskAction Action { get; set; }
        internal Dictionary<string, object> Context { get; set; }

        internal TaskArgs()
        {
            Context = new Dictionary<string, object>();
        }

        internal enum TaskAction
        {
            /// <summary>
            /// The task is executed prior to installation/update.
            /// </summary>
            Pre,
            /// <summary>
            /// The task is executed prior to installation/update.
            /// </summary>
            Installation,
            /// <summary>
            /// The task is executed after installation/update.
            /// </summary>
            Post,
        }
    }
}
