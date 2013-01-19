using System;
using System.Collections.Generic;
using System.Linq;
using AlarmWorkflow.AlarmSource.Fax;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Parser.ILSFFBParser
{
    /// <summary>
    /// Description of ILSFFBParser.
    /// </summary>
    [Export("ILSFFBParser", typeof(IFaxParser))]
    public class ILSFFBParser : IFaxParser
    {
        private readonly Dictionary<string, string> _fdUnits;
        private readonly string[] _keywords = new[]
                                                        {
                                                            "ALARM","E-Nr","EINSATZORT","STRAﬂE",
                                                            "ORTSTEIL/ORT","OBJEKT","EINSATZPLAN","MELDEBILD",
                                                            "EINSATZSTICHWORT","HINWEIS","EINSATZMITTEL","(ALARMSCHREIBEN ENDE)"
                                                        };
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ILSFFBParser class.
        /// </summary>
        public ILSFFBParser()
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

        #endregion

        #region IFaxParser Members

        Operation IFaxParser.Parse(string[] lines)
        {
            Operation operation = new Operation();
            CurrentSection section = CurrentSection.AAnfang;
            lines = Utilities.Trim(lines);
            foreach (var line in lines)
            {
                string keyword;
                if (GetKeyword(line, out keyword))
                {
                    switch (keyword.Trim())
                    {
                        case "E-Nr": { section = CurrentSection.BeNr; break; }
                        case "EINSATZORT": { section = CurrentSection.CEinsatzort; break; }
                        case "STRAﬂE": { section = CurrentSection.DStraﬂe; break; }
                        case "ORTSTEIL/ORT": { section = CurrentSection.EOrt; break; }
                        case "OBJEKT": { section = CurrentSection.FObjekt; break; }
                        case "EINSATZPLAN": { section = CurrentSection.GEinsatzplan; break; }
                        case "MELDEBILD": { section = CurrentSection.HMeldebild; break; }
                        case "EINSATZSTICHWORT": { section = CurrentSection.JEinsatzstichwort; break; }
                        case "HINWEIS": { section = CurrentSection.KHinweis; break; }
                        case "EINSATZMITTEL": { section = CurrentSection.LEinsatzmittel; break; }
                        case "(ALARMSCHREIBEN ENDE)": { section = CurrentSection.MEnde; break; ;}
                    }
                }


                switch (section)
                {
                    case CurrentSection.BeNr:
                        operation.OperationNumber = GetMessageText(line.Substring(0, line.IndexOf("ALARM", StringComparison.Ordinal)), keyword);
                        keyword = "ALARM";
                        try
                        {
                            operation.Timestamp = DateTime.Parse(GetMessageText(line.Substring(line.IndexOf("ALARM", StringComparison.Ordinal)), keyword));
                        }
                        catch (FormatException)
                        {
                            operation.Timestamp = DateTime.Now;
                        }
                        break;
                    case CurrentSection.CEinsatzort:
                        operation.Zielort.Location = GetMessageText(line, keyword);
                        break;
                    case CurrentSection.DStraﬂe:
                        operation.Einsatzort.Street = GetMessageText(line, keyword);
                        break;
                    case CurrentSection.EOrt:
                        operation.Einsatzort.City = GetMessageText(line, keyword);
                        break;
                    case CurrentSection.FObjekt:
                        operation.Einsatzort.Property = GetMessageText(line, keyword);
                        break;
                    case CurrentSection.GEinsatzplan:
                        operation.OperationPlan = GetMessageText(line, keyword);
                        break;
                    case CurrentSection.HMeldebild:
                        operation.Picture = GetMessageText(line, keyword);
                        break;
                    case CurrentSection.JEinsatzstichwort:
                        operation.Keywords.EmergencyKeyword = GetMessageText(line, keyword);
                        break;
                    case CurrentSection.KHinweis:
                        operation.Comment += GetMessageText(line, keyword);
                        break;
                    case CurrentSection.LEinsatzmittel:
                        if (line.Equals("EINSATZMITTEL: ", StringComparison.InvariantCultureIgnoreCase))
                        {
                            break;
                        }
                        OperationResource resource = new OperationResource();
                        if (line.Contains('('))
                        {
                            string tool = line.Substring(line.IndexOf("(", StringComparison.Ordinal) + 1);
                            if (tool.Length >= 2)
                            {
                                tool = tool.Substring(0, tool.Length - 2).Trim();
                            }
                            else
                            {
                                tool = String.Empty;
                            }
                            string unit = line.Substring(0, line.IndexOf("(", StringComparison.Ordinal));
                            resource.FullName = unit;
                            resource.RequestedEquipment.Add(tool);
                            foreach (KeyValuePair<string, string> fdUnit in _fdUnits)
                            {
                                if (resource.FullName.ToLower().Contains(fdUnit.Key.ToLower()))
                                {
                                    resource.FullName = fdUnit.Value;
                                    operation.Resources.Add(resource);
                                    break;
                                }
                            }
                        }
                        break;
                        
                    case CurrentSection.MEnde:
                        break;

                }
            }

            return operation;
        }

        #endregion

        #region Methods
        private bool GetKeyword(string line, out string keyword)
        {
            line = line.ToUpperInvariant();
            foreach (string kwd in _keywords.Where(kwd => line.ToLowerInvariant().StartsWith(kwd.ToLowerInvariant())))
            {
                keyword = kwd;
                return true;
            }
            keyword = null;
            return false;
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
                line = line.Remove(0, prefix.Length);
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
                line = line.Remove(0, 1);
            }
            line = line.Trim();
            return line;
        }

        #endregion

        #region Nested types

        private enum CurrentSection
        {
            AAnfang,
            BeNr,
            CEinsatzort,
            DStraﬂe,
            EOrt,
            FObjekt,
            GEinsatzplan,
            HMeldebild,
            JEinsatzstichwort,
            KHinweis,
            LEinsatzmittel,
            MEnde
        }

        #endregion

    }
}
