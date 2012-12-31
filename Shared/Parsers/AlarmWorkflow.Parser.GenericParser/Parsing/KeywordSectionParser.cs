using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Parser.GenericParser.Parsing
{
    /// <summary>
    /// Provides a section parser that parses a section containing keywords.
    /// </summary>
    [Export("KeywordSectionParser", typeof(ISectionParser))]
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
