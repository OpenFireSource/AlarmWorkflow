using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AlarmWorkflow.AlarmSource.Fax;
using AlarmWorkflow.Parser.GenericParser.Misc;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Parser.GenericParser
{
    /// <summary>
    /// Represents the logic of a parser that parses faxes based on simple rules for simple faxes.
    /// </summary>
    [Export("GenericParser", typeof(IFaxParser))]
    class GenericParser : IFaxParser
    {
        #region Fields

        private Configuration _configuration;
        private ControlInformation _controlInformation;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericParser"/> class.
        /// </summary>
        public GenericParser()
        {
            _configuration = new Configuration();
            LoadControlInformationFile();
        }

        #endregion

        #region Methods

        private void LoadControlInformationFile()
        {
            string fileName = _configuration.ControlFile;
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

        #region IFaxParser Members

        Operation IFaxParser.Parse(string[] lines)
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
