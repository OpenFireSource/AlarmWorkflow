using System;

namespace AlarmWorkflow.Tools.MakeUpdatePackage.Tasks
{
    interface ITask
    {
        void Execute(Context context);
    }
}
