using System.Windows;
using System.Windows.Controls;
using AlarmWorkflow.Parser.GenericParser.ViewModels;

namespace AlarmWorkflow.Parser.GenericParser.Views
{
    /// <summary>
    /// Interaction logic for ParserTreeView.xaml
    /// </summary>
    public partial class ParserTreeView : UserControl
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserTreeView"/> class.
        /// </summary>
        public ParserTreeView()
        {
            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ParserDefinitionViewModel vm = (ParserDefinitionViewModel)this.DataContext;
            vm.SelectedNode = e.NewValue;
        }

        #endregion
    }
}
