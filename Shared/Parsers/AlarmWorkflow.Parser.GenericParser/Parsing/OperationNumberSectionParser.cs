using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Parser.GenericParser.Parsing
{
    /// <summary>
    /// Provides a section parser that parses a section containing the operation number.
    /// </summary>
    [Export("OperationNumberSectionParser", typeof(ISectionParser))]
    [Information(DisplayName = "OperationNumberSectionParser_DisplayName", Description = "OperationNumberSectionParser_Description")]
    public class OperationNumberSectionParser : ISectionParser
    {
        #region ISectionParser Members

        void ISectionParser.OnLoad(System.Collections.Generic.IDictionary<string, string> parameters)
        {

        }

        void ISectionParser.OnSave(System.Collections.Generic.IDictionary<string, string> parameters)
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
