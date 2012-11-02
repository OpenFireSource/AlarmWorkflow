using System.Collections.Generic;
using System.Diagnostics;
using AlarmWorkflow.Windows.UI.ViewModels;
using AlarmWorkflow.Windows.Configuration.Config;

namespace AlarmWorkflow.Windows.Configuration.ViewModels
{
    [DebuggerDisplay("Section = {Identifier}")]
    class SectionViewModel : ViewModelBase
    {
        #region Fields

        private IdentifierInfo _identifier;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier of this section.
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// Gets the text to display in the UI.
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (_identifier == null || string.IsNullOrWhiteSpace(_identifier.DisplayText))
                {
                    return Identifier;
                }
                return _identifier.DisplayText;
            }
        }
        /// <summary>
        /// Gets a list of all setting item VMs.
        /// </summary>
        public List<SettingItemViewModel> SettingItems { get; private set; }

        #endregion

        #region Constructors

        private SectionViewModel()
        {
            SettingItems = new List<SettingItemViewModel>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionViewModel"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public SectionViewModel(IdentifierInfo identifier)
            : this()
        {
            _identifier = identifier;
        }

        #endregion
    }
}
