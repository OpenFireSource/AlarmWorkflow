using System.Windows;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Windows.UIContracts.Extensibility
{
    /// <summary>
    /// Defines a means for a type that provides a parser-specific frontend for an Operation.
    /// </summary>
    public interface IOperationViewer
    {
        /// <summary>
        /// Gets the <see cref="FrameworkElement"/> which is used to represent the operation.
        /// </summary>
        FrameworkElement Visual { get; }
        /// <summary>
        /// Called when a new operation is coming in, prior to it <see cref="OnOperationChanged(Operation)"/> being called.
        /// </summary>
        /// <param name="operation">The new operation that is about being changed over to.</param>
        void OnNewOperation(Operation operation);
        /// <summary>
        /// Called when the selected operation has changed and we need to display the given one.
        /// </summary>
        /// <param name="operation">The <see cref="Operation"/> that was selected and is now being displayed.</param>
        void OnOperationChanged(Operation operation);
    }
}
