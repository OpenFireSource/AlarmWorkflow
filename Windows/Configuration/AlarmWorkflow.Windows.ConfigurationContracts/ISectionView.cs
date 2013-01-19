using System;
using System.Windows;

namespace AlarmWorkflow.Windows.ConfigurationContracts
{
    /// <summary>
    /// Defines a view on a section.
    /// </summary>
    public interface ISectionView
    {
        /// <summary>
        /// Returns the visual element representing this view.
        /// </summary>
        UIElement Visual { get; }

        /// <summary>
        /// Saves the settings in this settings.
        /// </summary>
        void Save();
    }
}
