using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AlarmWorkflow.Parser.GenericParser.Control;
using AlarmWorkflow.Parser.GenericParser.Misc;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using System.Diagnostics;

namespace AlarmWorkflow.Parser.GenericParser.Parsing
{
    sealed class ParserInstance
    {
        #region Fields

        private readonly ControlInformation _controlInformation;
        private readonly List<SectionData> _sections;

        #endregion

        #region Constructors

        internal ParserInstance(ControlInformation controlInformation)
        {
            Assertions.AssertNotNull(controlInformation, "controlInformation");

            _controlInformation = controlInformation;

            _sections = new List<SectionData>();
        }

        #endregion

        #region Methods

        internal Operation Parse(string[] lines)
        {
            BuildSections(lines);
            TokenizeLines();

            Operation operation = new Operation();

            // Iterate over each section and its areas
            foreach (SectionData section in _sections)
            {
                // Enter section for all parsers
                section.Parsers.ForEach(p => p.OnEnterSection(operation));

                foreach (AreaToken areaToken in section.Tokens)
                {
                    AreaDefinition area = section.Definition.Areas.Find(tmp => tmp.AreaString.String == areaToken.Identifier);
                    if (area != null)
                    {
                        WriteValueToOperation(areaToken.Value, area, operation);
                    }
                    section.Parsers.ForEach(p => p.Populate(areaToken, operation));
                }


                // Exit section for all parsers
                section.Parsers.ForEach(p => p.OnLeaveSection(operation));
            }

            return operation;
        }

        /// <summary>
        /// Processes all lines based on the control information and builds the cache data for later.
        /// </summary>
        /// <param name="lines"></param>
        private void BuildSections(string[] lines)
        {
            SectionData currentSection = null;
            for (int i = 0; i < lines.Length; i++)
            {
                try
                {
                    string line = lines[i];
                    if (line.Length == 0)
                    {
                        continue;
                    }

                    // Check if the line denotes a section start. In this case, skip this line but remember the section.
                    SectionDefinition newSection = null;
                    if (IsLineSectionMarker(line, out newSection))
                    {
                        // Only switch to new section if it is a different section!
                        if (currentSection == null || (newSection.SectionString.String != currentSection.Definition.SectionString.String))
                        {
                            // Create new section
                            SectionData sectionData = new SectionData(newSection);
                            _sections.Add(sectionData);

                            currentSection = sectionData;

                            continue;
                        }
                    }

                    // Check if we currently are in no section (this is the case when there is some jabber at the beginning we don't care about).
                    if (currentSection == null)
                    {
                        continue;
                    }

                    // Add to current section
                    currentSection.Lines.Add(line);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, "Error while parsing line '{0}'. The error message was: {1}", i, ex.Message);
                }
            }
        }

        private void TokenizeLines()
        {
            foreach (SectionData section in _sections)
            {
                section.TokenizeLines();
            }
        }

        private bool IsLineSectionMarker(string line, out SectionDefinition currentSection)
        {
            currentSection = _controlInformation.GetSection(line);
            return currentSection != null;
        }

        private bool IsLineAreaRelevant(string line, SectionDefinition currentSection, out AreaDefinition area)
        {
            area = currentSection.GetArea(line);
            return area != null;
        }

        private void WriteValueToOperation(string value, AreaDefinition area, Operation operation)
        {
            // First check: Check if the object has the property we want right away
            bool propertyFound = WriteValueToProperty(value, area.MapToPropertyExpression, operation);
            if (propertyFound)
            {
                Logger.Instance.LogFormat(LogType.Debug, this, "Wrote value '{0}' successfully to property '{1}'.", value, area.MapToPropertyExpression);
            }
            else
            {
                PropertyInfo piCustomData = operation.GetType().GetProperty("CustomData");
                WriteValueToCustomData(value, area, piCustomData, operation);
            }
        }

        private bool WriteValueToProperty(string value, string path, Operation operation)
        {
            bool success = false;
            try
            {
                // TODO: Use class ObjectExpressionTools!
                success = Helpers.SetValueFromExpression(operation, path, value);
            }
            catch (Exception)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "Could not write area to mapped property '{0}'. Please check for typos and a correct mapping!", path);
                throw;
            }

            return success;
        }

        private void WriteValueToCustomData(string value, AreaDefinition area, PropertyInfo piCustomData, Operation operation)
        {
            // We know that the CustomData-object is a dictionary with string, object...
            IDictionary<string, object> customData = (IDictionary<string, object>)piCustomData.GetValue(operation, null);

            // If the custom data entry already exists, give a warning and overwrite it
            if (customData.ContainsKey(area.MapToPropertyExpression))
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "The custom data entry '{0}' does already exist and is being overwritten. Please check your control file if this is the intended behavior!", area.MapToPropertyExpression);
            }
            customData[area.MapToPropertyExpression] = value;

            Logger.Instance.LogFormat(LogType.Debug, this, "Wrote value '{0}' successfully to custom data entry '{1}'.", value, area.MapToPropertyExpression);
        }

        #endregion

        #region Nested types

        [DebuggerDisplay("Name = {Definition.SectionString}")]
        class SectionData
        {
            internal SectionDefinition Definition { get; private set; }

            internal List<string> Lines { get; set; }
            internal List<AreaToken> Tokens { get; set; }
            internal List<ISectionParser> Parsers { get; set; }

            internal SectionData(SectionDefinition definition)
            {
                Assertions.AssertNotNull(definition, "definition");

                Definition = definition;

                Lines = new List<string>();
                InitializeParsers();
            }

            private void InitializeParsers()
            {
                Parsers = new List<ISectionParser>();
                foreach (SectionParserDefinition spd in Definition.Parsers)
                {
                    ISectionParser parser = SectionParserCache.Create(spd.Type);
                    parser.OnLoad(spd.Options);
                    Parsers.Add(parser);
                }
            }

            internal void TokenizeLines()
            {
                IList<string> validTokens = GetValidTokens();

                // Merge all lines to check for tokens
                string mergedText = string.Join("", Lines);

                Tokens = new List<AreaToken>();
                foreach (string line in Lines)
                {
                    IList<AreaToken> tokensInThisLine = GetTokensInLine(validTokens, line).ToList();
                    if (tokensInThisLine.Count == 0)
                    {
                        // No token(s) --> take whole line as one token with nothing else
                        AreaToken areaToken = new AreaToken(line);
                        Tokens.Add(areaToken);
                    }

                    Tokens.AddRange(tokensInThisLine);
                }
            }

            private static IEnumerable<AreaToken> GetTokensInLine(IList<string> validTokens, string line)
            {
                List<AreaToken> tokens = new List<AreaToken>();

                foreach (string tokenInQuestion in validTokens)
                {
                    int index = line.IndexOf(tokenInQuestion, StringComparison.CurrentCultureIgnoreCase);
                    if (index == -1)
                    {
                        continue;
                    }

                    AreaToken at = new AreaToken(line);
                    at.Identifier = tokenInQuestion;
                    at.Occurrence = index;

                    tokens.Add(at);
                }

                // Second step: Parse the values, knowing successors
                LinkedList<AreaToken> linkedList = new LinkedList<AreaToken>(tokens);

                LinkedListNode<AreaToken> currentToken = linkedList.First;
                while (currentToken != null)
                {
                    int from = currentToken.Value.Occurrence;
                    int to = currentToken.Next != null ? currentToken.Next.Value.Occurrence : 0;

                    currentToken.Value.Value = GetMessageText(line, currentToken.Value.Identifier, from, to);

                    if (currentToken.Next == null)
                    {
                        break;
                    }
                    currentToken = currentToken.Next;
                }

                return linkedList;
            }

            private IList<string> GetValidTokens()
            {
                List<string> validTokens = new List<string>();
                validTokens.AddRange(Definition.Areas.Select(a => a.AreaString.String));
                validTokens.AddRange(Parsers.SelectMany(p => p.GetTokens()));
                return validTokens;
            }

            private static string GetMessageText(string line, string prefix, int from, int to)
            {
                if (from > 0)
                {
                    line = line.Remove(0, from);
                }
                if (to > 0)
                {
                    line = line.Substring(0, to);
                }

                if (prefix == null)
                {
                    prefix = "";
                }

                if (prefix.Length > 0)
                {
                    line = line.Remove(0, prefix.Length).Trim();
                }
                else
                {
                    int colonIndex = line.IndexOf(':');
                    if (colonIndex != -1)
                    {
                        line = line.Remove(0, colonIndex + 1);
                    }
                }

                if (line.StartsWith(":"))
                {
                    line = line.Remove(0, 1).Trim();
                }

                return line;
            }
        }

        #endregion

    }
}
