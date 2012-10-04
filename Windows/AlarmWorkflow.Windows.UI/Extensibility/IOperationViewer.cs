using System;
using System.Windows;

namespace AlarmWorkflow.Windows.UI.Extensibility
{
    /// <summary>
    /// Defines a means for a type that provides a parser-specific frontend for an Operation.
    /// </summary>
    public interface IOperationViewer
    {
        /// <summary>
        /// Returns a new instance of an <see cref="FrameworkElement"/> which is used to represent the operation.
        /// </summary>
        /// <returns></returns>
        FrameworkElement CreateTemplate();
    }
}
