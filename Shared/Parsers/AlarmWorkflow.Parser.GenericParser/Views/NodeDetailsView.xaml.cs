using System.Windows;
using System.Windows.Controls;
using AlarmWorkflow.Parser.GenericParser.Parsing;
using AlarmWorkflow.Parser.GenericParser.ViewModels;

namespace AlarmWorkflow.Parser.GenericParser.Views
{
    /// <summary>
    /// Interaction logic for NodeDetailsView.xaml
    /// </summary>
    public partial class NodeDetailsView : UserControl
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeDetailsView"/> class.
        /// </summary>
        public NodeDetailsView()
        {
            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void AddSectionParserMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Put in MVVM
            // Get original menu item and create new parser in the current section
            string type = (string)((MenuItem)e.OriginalSource).Header;

            SectionDefinitionViewModel vm = (SectionDefinitionViewModel)((ParserDefinitionViewModel)this.DataContext).SelectedNode;
            ISectionParser parser = SectionParserCache.Create(type);

            SectionParserDefinitionViewModel svm = new SectionParserDefinitionViewModel(vm, parser);
            vm.Aspects.Add(svm);
        }

        #endregion

    }
}
