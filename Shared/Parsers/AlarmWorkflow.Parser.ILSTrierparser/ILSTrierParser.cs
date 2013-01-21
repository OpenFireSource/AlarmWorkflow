using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AlarmWorkflow.AlarmSource.Fax;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Parser.ILSTrierParser
{
    /// <summary>
    /// Provides a parser that parses faxes from the ILSTrierParser.
    /// </summary>
    [Export("ILSTrierParser", typeof(IFaxParser))]
    public class ILSTrierParser : IFaxParser
    {
        private readonly Dictionary<string, string> _fdUnits;

        public ILSTrierParser()
        {
            _fdUnits = new Dictionary<string, string>();
            string[] units = SettingsManager.Instance.GetSetting("Shared", "FD.Units").GetStringArray();
            foreach (string unit in units)
            {
                string[] result = unit.Split(new[] { "=;=" }, StringSplitOptions.None);
                if (result.Length == 2)
                {
                    _fdUnits.Add(result[0], result[1]);
                }
                else
                {
                    _fdUnits.Add(unit, unit);
                }
            }
        }

        Operation IFaxParser.Parse(string[] lines)
        {
            Operation operation = new Operation();
            lines = Utilities.Trim(lines);
            CurrentSection section = CurrentSection.AHeader;
            for (int index = 0; index < lines.Length; index++)
            {
                string line = lines[index];
                if (line.ToUpper().StartsWith("ALARMPLAN"))
                {
                    String temp = line.Substring(line.ToUpper().IndexOf("EINSATZNUMMER", StringComparison.Ordinal)).Trim();
                    temp = temp.Substring(temp.IndexOf(" ", StringComparison.Ordinal)).Trim();
                    operation.OperationNumber = temp.Substring(0, temp.IndexOf(" ", StringComparison.Ordinal)).Trim();
                    section = CurrentSection.BBody;
                }
                else if (line.ToUpper().StartsWith("PLZ ORT"))
                {
                    operation.Einsatzort.City = GetMessageText(line, "PLZ ORT");
                }
                else if (line.ToUpper().StartsWith("STRASSE"))
                {
                    operation.Einsatzort.Street = GetMessageText(line, "STRASSE");
                }
                else if (line.ToUpper().StartsWith("OBJEKT"))
                {
                    operation.Einsatzort.Property = GetMessageText(line, "OBJEKT");
                }
                else if (line.ToUpper().StartsWith("TELEFON"))
                {
                    operation.Messenger = GetMessageText(line, "TELEFON");
                }
                else if (line.ToUpper().StartsWith("MELDUNG INFO"))
                {
                    operation.Comment = GetMessageText(line, "MELDUNG INFO");
                }
                else if (line.ToUpper().StartsWith("EINSATZART"))
                {
                    operation.Keywords.Keyword = GetMessageText(line, "EINSATZART");
                }
                else if (line.ToUpper().StartsWith("STICHWORT"))
                {
                    operation.Keywords.EmergencyKeyword = GetMessageText(line, "STICHWORT");
                }
                else if (line.ToUpper().StartsWith("ANFAHRT"))
                {
                    section = CurrentSection.CAnfahrt;
                    continue;
                }
                else if (line.ToUpper().StartsWith("BETEILIGTE EINSATZMITTEL"))
                {
                    section = CurrentSection.DEinsatzMittel;
                    continue;
                }
                else if (line.ToUpper().StartsWith("AP"))
                {
                    section = CurrentSection.EFooter;
                }
                switch (section)
                {
                    case CurrentSection.CAnfahrt:
                        if (line.Contains("PLZ"))
                        {
                            operation.OperationPlan = line.Substring(0, line.ToUpper().IndexOf("PLZ", StringComparison.Ordinal));
                            String temp = line.Substring(line.ToUpper().IndexOf("PLZ"));
                            if (temp.Contains(" "))
                            {
                                temp = temp.Substring(temp.IndexOf(" "));
                            }
                            operation.Einsatzort.ZipCode = temp.Trim();
                        }
                        else
                        {
                            operation.OperationPlan = line;
                        }
                        //Only the first line is interesting ;)
                        section = CurrentSection.BBody;
                        break;
                    case CurrentSection.DEinsatzMittel:
                        if (line.ToUpper().Contains("FAHRZEUG"))
                        {
                            continue;
                        }

                        Regex timeStamp = new Regex("\\d\\d:\\d\\d:\\d\\d");
                        line = timeStamp.Replace(line, "").Trim();
                        OperationResource resource = new OperationResource { FullName = line };
                        foreach (KeyValuePair<string, string> fdUnit in _fdUnits)
                        {
                            if (resource.FullName.ToLower().Contains(fdUnit.Key.ToLower()))
                            {
                                resource.FullName = fdUnit.Value;
                                operation.Resources.Add(resource);
                                break;
                            }
                        }
                        operation.Resources.Add(resource);
                        break;
                    case CurrentSection.EFooter:
                        return operation;
                }
            }
            return operation;
        }

        /// <summary>
        /// Returns the message text, which is the line text but excluding the keyword/prefix and a possible colon.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="prefix">The prefix that is to be removed (optional).</param>
        /// <returns></returns>
        private string GetMessageText(string line, string prefix)
        {
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
        #region Nested types

        private enum CurrentSection
        {
            AHeader,
            BBody,
            CAnfahrt,
            DEinsatzMittel,
            EFooter,
        }

        #endregion
    }
}