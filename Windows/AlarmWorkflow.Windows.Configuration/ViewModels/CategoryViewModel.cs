using System.Collections.Generic;
using AlarmWorkflow.Windows.UI.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.ViewModels
{
    class CategoryViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets/sets the name of the category.
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// Gets a list of all setting item VMs.
        /// </summary>
        public List<SettingItemViewModel> SettingItems { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryViewModel"/> class.
        /// </summary>
        public CategoryViewModel()
        {
            SettingItems = new List<SettingItemViewModel>();
        }

        #endregion
    }
}
