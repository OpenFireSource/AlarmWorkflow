using System.Collections.Generic;
using System.Reflection;
using AlarmWorkflow.Parser.GenericParser.Parsing;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Parser.GenericParser.ViewModels
{
    class SectionParserDefinitionViewModel : ViewModelBase
    {
        #region Fields

        private SectionDefinitionViewModel _parent;
        private ISectionParser _parser;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the type name of the section parser.
        /// </summary>
        public string Type
        {
            get { return _parser.GetType().Name; }
        }
        /// <summary>
        /// Gets the list of options for this section parser.
        /// </summary>
        public List<OptionViewModel> Options { get; private set; }

        /// <summary>
        /// Gets the underlying parser instance.
        /// </summary>
        public ISectionParser Parser
        {
            get { return _parser; }
        }

        #endregion

        #region Commands

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionParserDefinitionViewModel"/> class.
        /// </summary>
        private SectionParserDefinitionViewModel()
        {
            Options = new List<OptionViewModel>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionParserDefinitionViewModel"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="sectionParser">The section parser to use.</param>
        public SectionParserDefinitionViewModel(SectionDefinitionViewModel parent, ISectionParser sectionParser)
            : this()
        {
            Assertions.AssertNotNull(parent, "parent");
            Assertions.AssertNotNull(sectionParser, "sectionParser");
            _parent = parent;
            _parser = sectionParser;

            CreateOptionVMs();
        }

        #endregion

        #region Methods

        private void CreateOptionVMs()
        {
            foreach (PropertyInfo property in _parser.GetType().GetProperties())
            {
                OptionAttribute[] oa = (OptionAttribute[])property.GetCustomAttributes(typeof(OptionAttribute), true);
                if (oa.Length == 0)
                {
                    continue;
                }

                OptionViewModel ovm = new OptionViewModel();
                ovm.Attribute = oa[0];
                ovm.Property = property;
                ovm.Instance = _parser;

                Options.Add(ovm);
            }
        }

        #endregion

        #region Nested types

        internal class OptionViewModel : ViewModelBase
        {
            public OptionAttribute Attribute { get; set; }
            public ISectionParser Instance { get; set; }
            public PropertyInfo Property { get; set; }

            public object Value
            {
                get { return Property.GetValue(Instance, null); }
                set { Property.SetValue(Instance, value, null); }
            }

            public string DisplayName
            {
                get
                {
                    if (!string.IsNullOrWhiteSpace(Attribute.DisplayName))
                    {
                        return Attribute.DisplayName;
                    }
                    return Property.Name;
                }
            }

            public string Description
            {
                get
                {
                    if (!string.IsNullOrWhiteSpace(Attribute.Description))
                    {
                        return Attribute.Description;
                    }
                    return Properties.Resources.NoDescriptionAvailableText;
                }
            }
        }

        #endregion
    }
}
