using System.Collections.Generic;
using System.Diagnostics;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Windows.UI.ViewModels;

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
        public List<CategoryViewModel> CategoryItems { get; private set; }

        #endregion

        #region Constructors

        private SectionViewModel()
        {
            CategoryItems = new List<CategoryViewModel>();
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

        #region Methods

        internal void Add(SettingDescriptor descriptor, SettingInfo setting)
        {
            CategoryViewModel cvm = CategoryItems.Find(c => c.Category == setting.Category);
            if (cvm == null)
            {
                cvm = new CategoryViewModel();
                cvm.Category = setting.Category;
                if (string.IsNullOrWhiteSpace(cvm.Category))
                {
                    cvm.Category = "(Sonstige)";
                }
                CategoryItems.Add(cvm);
            }

            cvm.SettingItems.Add(new SettingItemViewModel(descriptor, setting));
        }

        #endregion

    }
}
