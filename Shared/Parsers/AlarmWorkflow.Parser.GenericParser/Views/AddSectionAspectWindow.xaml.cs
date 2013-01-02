using System.Windows;
using System.Collections.Generic;
using System;
using AlarmWorkflow.Parser.GenericParser.Parsing;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Parser.GenericParser.Views
{
    /// <summary>
    /// Interaction logic for AddSectionAspectWindow.xaml
    /// </summary>
    internal partial class AddSectionAspectWindow : Window
    {
        #region Properties

        /// <summary>
        /// Gets/sets a list containing all aspect types to display.
        /// </summary>
        public List<AspectViewModel> AspectTypes { get; private set; }
        /// <summary>
        /// Gets/sets the aspect type that was selected.
        /// </summary>
        public AspectViewModel SelectedType { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddSectionAspectWindow"/> class.
        /// </summary>
        public AddSectionAspectWindow()
        {
            InitializeComponent();

            AspectTypes = new List<AspectViewModel>();
            foreach (Type type in SectionParserCache.Types)
            {
                AspectViewModel vm = new AspectViewModel();
                vm.Type = type.Name;
                vm.DisplayName = InformationAttribute.GetDisplayName(type);
                vm.Description = InformationAttribute.GetDescription(type);

                AspectTypes.Add(vm);
            }

            SelectedType = AspectTypes[0];

            this.DataContext = this;
        }

        #endregion

        #region Event handlers

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #endregion

        #region Nested types

        internal class AspectViewModel
        {
            public string DisplayName { get; set; }
            public string Description { get; set; }
            public string Type { get; set; }
        }

        #endregion
    }
}
