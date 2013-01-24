using System.Collections.Generic;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.ViewModels
{
    class GroupedSectionViewModel : ViewModelBase
    {
        #region Properties

        public List<GroupedSectionViewModel> Children { get; private set; }
        public SectionViewModel Section { get; private set; }

        public bool IsSelected { get; set; }
        public string Identifier { get; set; }
        public string Header { get; set; }
        public int Order { get; set; }

        #endregion

        #region Constructors

        internal GroupedSectionViewModel(SectionViewModel section)
        {
            Children = new List<GroupedSectionViewModel>();

            if (section != null)
            {
                Section = section;

                Identifier = Section.Identifier;
                Header = Section.DisplayText;
                Order = section.Order;
            }
        }

        #endregion
    }
}
