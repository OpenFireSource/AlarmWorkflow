using System.Windows;
using AlarmWorkflow.Shared.Core;
using AvalonDock.Layout.Serialization;

namespace AlarmWorkflow.Windows.CustomViewer.Extensibility
{
    /// <summary>
    ///     Defines a new IUIWidget.
    /// </summary>
    public interface IUIWidget
    {
        /// <summary>
        ///     Gets the <see cref="UIElement" /> which is used to represent the Widget.
        /// </summary>
        UIElement UIElement { get; }

        /// <summary>
        ///     Gets the ContentGUID of the widget, which is required for the <see cref="XmlLayoutSerializer" /> (should be uinque)
        /// </summary>
        string ContentGuid { get; }

        /// <summary>
        ///     Gets the Title of the widget
        /// </summary>
        string Title { get; }

        /// <summary>
        ///     Initializes this Widget
        /// </summary>
        /// <returns>The result of the initialization. Widget that return false won't be called.</returns>
        bool Initialize();

        /// <summary>
        ///     Called when the selected operation has changed and we need to display the given one.
        /// </summary>
        /// <param name="operation">
        ///     The <see cref="Operation" /> that was selected and is now being displayed.
        /// </param>
        void OnOperationChange(Operation operation);
    }
}