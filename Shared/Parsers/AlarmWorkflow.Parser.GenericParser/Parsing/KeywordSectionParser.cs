using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Parser.GenericParser.Parsing
{
    /// <summary>
    /// Provides a section parser that parses a section containing keywords.
    /// </summary>
    [Export("KeywordSectionParser", typeof(ISectionParser))]
    [Information(DisplayName = "KeywordSectionParser_DisplayName", Description = "KeywordSectionParser_Description")]
    public class KeywordSectionParser : ISectionParser
    {
        #region Properties

        [Option("Schlagwort")]
        public string KeywordKeyword { get; set; }
        [Option("Stichwort B")]
        public string KeywordB { get; set; }
        [Option("Stichwort R")]
        public string KeywordR { get; set; }
        [Option("Stichwort S")]
        public string KeywordS { get; set; }
        [Option("Stichwort T")]
        public string KeywordT { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeywordSectionParser"/> class.
        /// </summary>
        public KeywordSectionParser()
        {
            KeywordKeyword = "Schlagw";
            KeywordB = "Stichwort B";
            KeywordR = "Stichwort R";
            KeywordS = "Stichwort S";
            KeywordT = "Stichwort T";
        }

        #endregion

        #region ISectionParser Members

        System.Collections.Generic.IEnumerable<string> ISectionParser.GetTokens()
        {
            yield return KeywordKeyword;
            yield return KeywordB;
            yield return KeywordR;
            yield return KeywordS;
            yield return KeywordT;
        }

        void ISectionParser.OnLoad(System.Collections.Generic.IDictionary<string, string> parameters)
        {
            KeywordKeyword = parameters.SafeGetValue("KeywordKeyword", KeywordKeyword);
            KeywordB = parameters.SafeGetValue("KeywordB", KeywordB);
            KeywordR = parameters.SafeGetValue("KeywordR", KeywordR);
            KeywordS = parameters.SafeGetValue("KeywordS", KeywordS);
            KeywordT = parameters.SafeGetValue("KeywordT", KeywordT);
        }

        void ISectionParser.OnSave(System.Collections.Generic.IDictionary<string, string> parameters)
        {
            parameters.Add("KeywordKeyword", KeywordKeyword);
            parameters.Add("KeywordB", KeywordB);
            parameters.Add("KeywordR", KeywordR);
            parameters.Add("KeywordS", KeywordS);
            parameters.Add("KeywordT", KeywordT);
        }

        void ISectionParser.OnEnterSection(Operation operation)
        {

        }

        void ISectionParser.OnLeaveSection(Operation operation)
        {

        }

        void ISectionParser.Populate(AreaToken token, Operation operation)
        {
            string msg = token.Value;
            if (token.Identifier == KeywordKeyword)
            {
                operation.Keywords.Keyword = msg;
            }
            else if (token.Identifier == KeywordB)
            {
                operation.Keywords.B = msg;
            }
            else if (token.Identifier == KeywordR)
            {
                operation.Keywords.R = msg;
            }
            else if (token.Identifier == KeywordS)
            {
                operation.Keywords.S = msg;
            }
            else if (token.Identifier == KeywordT)
            {
                operation.Keywords.T = msg;
            }
        }

        #endregion
    }
}
