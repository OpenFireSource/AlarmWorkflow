using System.Collections.Generic;

namespace AlarmWorkflow.Windows.ConfigurationContracts
{
    /// <summary>
    /// Defines a means to provide sections for editing.
    /// </summary>
    public interface ISectionsProvider
    {
        /// <summary>
        /// Returns an enumerable containing all <see cref="ISectionView"/>s that this provider provides.
        /// </summary>
        /// <returns>An enumerable containing all <see cref="ISectionView"/>s that this provider provides.</returns>
        IEnumerable<ISectionView> GetViews();
    }
}
