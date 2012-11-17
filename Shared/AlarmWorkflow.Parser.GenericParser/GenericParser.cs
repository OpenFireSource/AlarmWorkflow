using System;
using System.IO;
using AlarmWorkflow.Parser.GenericParser.Misc;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Settings;
using System.Reflection;

namespace AlarmWorkflow.Parser.GenericParser
{
    /// <summary>
    /// Represents the logic of a parser that parses faxes based on simple rules for simple faxes.
    /// </summary>
    [Export("GenericParser", typeof(IParser))]
    class GenericParser : IParser
    {
        #region Fields

        private ControlInformation _controlInformation;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericParser"/> class.
        /// </summary>
        public GenericParser()
        {
            LoadControlInformationFile();
        }

        #endregion

        #region Methods

        private void LoadControlInformationFile()
        {
            string fileName = SettingsManager.Instance.GetSetting("GenericParser", "ControlFile").GetString();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            if (!Path.IsPathRooted(fileName))
            {
                fileName = Path.Combine(Utilities.GetWorkingDirectory(), fileName);
            }

            _controlInformation = ControlInformation.Load(fileName);
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
            PropertyInfo piLocal = operation.GetType().GetProperty(area.MapToPropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (piLocal != null)
            {
                WriteValueToProperty(value, piLocal, operation);
            }
            else
            {
                // TODO: This goes into the custom data instead!
            }
        }

        private void WriteValueToProperty(string value, PropertyInfo property, Operation operation)
        {
            try
            {
                property.SetValue(operation, value);

                Logger.Instance.LogFormat(LogType.Debug, this, "Wrote value '{0}' successfully to property '{1}'.", value, property.Name);
            }
            catch (Exception)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, "Could not write area to mapped property '{0}'. Please check for typos and a correct mapping!", property.Name);
                throw;
            }
        }

        #endregion

        #region IParser Members

        Operation IParser.Parse(string[] lines)
        {
            Operation operation = new Operation();

            lines = Utilities.Trim(lines);

            // Remember the name of the section we are in
            SectionDefinition currentSection = null;
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
                        currentSection = newSection;
                        continue;
                    }

                    // Check if we currently are in no section (this is the case when there is some jabber at the beginning we don't care about).
                    if (currentSection == null)
                    {
                        continue;
                    }

                    AreaDefinition area = null;
                    if (!IsLineAreaRelevant(line, currentSection, out area))
                    {
                        continue;
                    }

                    AreaLineInformation value = new AreaLineInformation(area, line);
                    WriteValueToOperation(value.Value, area, operation);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, "Error while parsing line '{0}'. The error message was: {1}", i, ex.Message);
                }
            }

            return operation;
        }

        #endregion
    }
}
