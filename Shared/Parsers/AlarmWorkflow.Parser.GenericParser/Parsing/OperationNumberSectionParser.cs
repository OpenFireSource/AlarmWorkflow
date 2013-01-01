using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Parser.GenericParser.Parsing
{
    /// <summary>
    /// Provides a section parser that parses a section containing the operation number.
    /// </summary>
    [Export("OperationNumberSectionParser", typeof(ISectionParser))]
    public class OperationNumberSectionParser : ISectionParser
    {
        #region ISectionParser Members

        void ISectionParser.OnLoad(System.Xml.Linq.XElement sectionElement)
        {

        }

        void ISectionParser.OnSave(System.Xml.Linq.XElement sectionElement)
        {

        }

        System.Collections.Generic.IEnumerable<string> ISectionParser.GetTokens()
        {
            yield break;
        }

        void ISectionParser.OnEnterSection(Operation operation)
        {

        }

        void ISectionParser.OnLeaveSection(Operation operation)
        {

        }

        void ISectionParser.Populate(AreaToken token, Operation operation)
        {
            if (token.Identifier == "Einsatznummer")
            {
                operation.OperationNumber = token.Value;
            }
        }

        #endregion
    }
}
