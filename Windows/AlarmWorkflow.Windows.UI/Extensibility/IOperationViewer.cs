using System.Windows;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Windows.UI.Extensibility
{
    /// <summary>
    /// Defines a means for a type that provides a parser-specific frontend for an Operation.
    /// </summary>
    public interface IOperationViewer
    {
        /// <summary>
        /// Returns a new instance of an <see cref="FrameworkElement"/> which is used to represent the operation.
        /// This method is called once during initialization of the UI.
        /// </summary>
        /// <returns></returns>
        FrameworkElement Create();
        /// <summary>
        /// Called when the selected operation has changed and we need to display the given one.
        /// </summary>
        /// <param name="operation">The <see cref="Operation"/> that was selected and is now being displayed.</param>
        void OnOperationChanged(Operation operation);
    }
}
