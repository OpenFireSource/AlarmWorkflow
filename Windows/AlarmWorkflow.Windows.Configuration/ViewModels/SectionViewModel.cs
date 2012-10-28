using System.Collections.Generic;
using System.Diagnostics;
using AlarmWorkflow.Windows.UI.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.ViewModels
{
    [DebuggerDisplay("Section = {Identifier}")]
    class SectionViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets the identifier of this section.
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// Gets a list of all setting item VMs.
        /// </summary>
        public List<SettingItemViewModel> SettingItems { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionViewModel"/> class.
        /// </summary>
        public SectionViewModel()
        {
            SettingItems = new List<SettingItemViewModel>();
        }

        #endregion
    }
}
