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
        /// <summary>
        /// Gets the Order of this section.
        /// </summary>
        public int Order
        {
            get { return _identifier.Order; }
        }

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
            string categoryText = GetCategoryText(setting.Category);

            CategoryViewModel cvm = CategoryItems.Find(c => c.Category == categoryText);
            if (cvm == null)
            {
                cvm = new CategoryViewModel();
                cvm.Category = categoryText;
                CategoryItems.Add(cvm);
            }

            cvm.SettingItems.Add(new SettingItemViewModel(descriptor, setting));
        }

        private string GetCategoryText(string categoryText)
        {
            return string.IsNullOrWhiteSpace(categoryText) ? "(Sonstige)" : categoryText;
        }

        #endregion

    }
}
