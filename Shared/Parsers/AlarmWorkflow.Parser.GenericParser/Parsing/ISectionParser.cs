using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Parser.GenericParser.Parsing
{
    /// <summary>
    /// Defines a method that is called for each line in a given section.
    /// </summary>
    public interface ISectionParser
    {
        /// <summary>
        /// Called when the section is entered for this parser.
        /// </summary>
        /// <param name="operation">The operation of the current context.</param>
        void OnEnterSection(Operation operation);
        /// <summary>
        /// Called when the section is left for this parser.
        /// This is the case when the next section is entered.
        /// </summary>
        /// <param name="operation">The operation of the current context.</param>
        void OnLeaveSection(Operation operation);

        /// <summary>
        /// Called by the parser for each line of the section this parser was defined for.
        /// </summary>
        /// <param name="token">The token to parse.</param>
        /// <param name="operation">The operation which to populate with the value behind the token.</param>
        void Populate(AreaToken token, Operation operation);
    }
}
