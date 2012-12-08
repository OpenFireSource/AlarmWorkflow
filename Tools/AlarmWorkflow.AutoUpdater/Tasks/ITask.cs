using System;

namespace AlarmWorkflow.Tools.AutoUpdater.Tasks
{
    interface ITask
    {
        void Execute(TaskArgs args);
    }
}
