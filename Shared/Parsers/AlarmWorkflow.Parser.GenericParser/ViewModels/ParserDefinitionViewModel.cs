using System.Collections.ObjectModel;
using System.Windows.Input;
using AlarmWorkflow.Parser.GenericParser.Control;
using AlarmWorkflow.Parser.GenericParser.Misc;
using AlarmWorkflow.Parser.GenericParser.Parsing;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Parser.GenericParser.ViewModels
{
    /// <summary>
    /// Defines the ViewModel that represents the concrete Parser definition.
    /// </summary>
    class ParserDefinitionViewModel : ViewModelBase
    {
        #region Fields

        private string _parserName;
        private object _selectedNode;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the name of the parser (optional).
        /// </summary>
        public string ParserName
        {
            get { return _parserName; }
            set
            {
                _parserName = value;
                OnPropertyChanged("ParserName");
            }
        }
        /// <summary>
        /// Gets/sets the keyword separator string (usually a colon).
        /// </summary>
        public string KeywordSeparator { get; set; }
        /// <summary>
        /// Gets/sets a collection of sections that make up the parser definition.
        /// </summary>
        public ObservableCollection<SectionDefinitionViewModel> Sections { get; set; }

        /// <summary>
        /// Gets/sets the selected item. This is a quick-hack for WPF's TreeView which cannot do this out of the box.
        /// </summary>
        public object SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                _selectedNode = value;
                OnPropertyChanged("SelectedNode");
            }
        }

        #endregion

        #region Commands

        #region Command "AddSectionCommand"

        /// <summary>
        /// The AddSectionCommand command.
        /// </summary>
        public ICommand AddSectionCommand { get; private set; }

        private void AddSectionCommand_Execute(object parameter)
        {
            SectionDefinitionViewModel section = new SectionDefinitionViewModel(this);
            Sections.Add(section);
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserDefinitionViewModel"/> class.
        /// </summary>
        public ParserDefinitionViewModel()
        {
            ParserName = Properties.Resources.ParserDefinitionBlankName;
            KeywordSeparator = ":";
            Sections = new ObservableCollection<SectionDefinitionViewModel>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Saves the parser definition to a file.
        /// </summary>
        /// <param name="fileName">The file to save the parser definition to.</param>
        public void Save(string fileName)
        {
            Assertions.AssertNotEmpty(fileName, "fileName");

            ControlInformation ci = new ControlInformation();
            ci.FaxName = this.ParserName;

            foreach (SectionDefinitionViewModel svm in this.Sections)
            {
                SectionDefinition sd = new SectionDefinition();
                sd.SectionString = new GenericParserString(svm.Name);

                foreach (SectionParserDefinitionViewModel spvm in svm.Aspects)
                {
                    SectionParserDefinition spd = new SectionParserDefinition();
                    spd.Type = spvm.Type;
                    spvm.Parser.OnSave(spd.Options);

                    sd.Parsers.Add(spd);
                }

                foreach (AreaDefinitionViewModel avm in svm.Areas)
                {
                    AreaDefinition ad = new AreaDefinition();
                    ad.AreaString = new GenericParserString(avm.Name);
                    ad.MapToPropertyExpression = avm.MapToPropertyExpression;

                    sd.Areas.Add(ad);
                }

                ci.Sections.Add(sd);
            }

            ci.Save(fileName);
        }

        /// <summary>
        /// Loads the parser definition from a file.
        /// </summary>
        /// <param name="fileName">The file to load the parser definition from.</param>
        public void Load(string fileName)
        {
            Assertions.AssertNotEmpty(fileName, "fileName");

            ControlInformation ci = ControlInformation.Load(fileName);

            // Clear all data
            this.Sections.Clear();
            this.SelectedNode = null;

            // Fill in from the control file...
            this.ParserName = ci.FaxName;

            foreach (SectionDefinition sd in ci.Sections)
            {
                SectionDefinitionViewModel sdvm = new SectionDefinitionViewModel(this);
                sdvm.Name = sd.SectionString.String;

                foreach (SectionParserDefinition spd in sd.Parsers)
                {
                    ISectionParser sectionParser = SectionParserCache.Create(spd.Type);
                    sectionParser.OnLoad(spd.Options);

                    SectionParserDefinitionViewModel spdvm = new SectionParserDefinitionViewModel(sdvm, sectionParser);

                    sdvm.Aspects.Add(spdvm);
                }
                foreach (AreaDefinition ad in sd.Areas)
                {
                    AreaDefinitionViewModel advm = new AreaDefinitionViewModel(sdvm);
                    advm.Name = ad.AreaString.String;
                    advm.MapToPropertyExpression = ad.MapToPropertyExpression;

                    sdvm.Areas.Add(advm);
                }

                this.Sections.Add(sdvm);
            }
        }

        #endregion

    }
}
