using System;
using System.Linq;
using AlarmWorkflow.AlarmSource.Fax;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Parser.ILSFFBParser
{
    /// <summary>
    /// Description of ILSFFBParser.
    /// </summary>
    [Export("ILSFFBParser", typeof(IFaxParser))]
    public class ILSFFBParser : IFaxParser
    {
        #region Fields
        
        private readonly string[] _keywords = new[]
                                                        {
                                                            "ALARM","E-Nr","EINSATZORT","STRAßE",
                                                            "ORTSTEIL/ORT","OBJEKT","EINSATZPLAN","MELDEBILD",
                                                            "EINSATZSTICHWORT","HINWEIS","EINSATZMITTEL","(ALARMSCHREIBEN ENDE)"
                                                        };
        
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
                        case "STRAßE": { section = CurrentSection.DStraße; break; }
                        case "ORTSTEIL/ORT": { section = CurrentSection.EOrt; break; }
                        case "OBJEKT": { section = CurrentSection.FObjekt; break; }
                        case "EINSATZPLAN": { section = CurrentSection.GEinsatzplan; break; }
                        case "MELDEBILD": { section = CurrentSection.HMeldebild; break; }
                        case "EINSATZSTICHWORT": { section = CurrentSection.JEinsatzstichwort; break; }
                        case "HINWEIS": { section = CurrentSection.KHinweis; break; }
                        case "EINSATZMITTEL": { section = CurrentSection.LEinsatzmittel; break; }
                        case "(ALARMSCHREIBEN ENDE)": { section = CurrentSection.MEnde; break;
                        }
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
                    case CurrentSection.DStraße:
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
                        operation.Comment += " " + GetMessageText(line, keyword);
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
                            tool = tool.Length >= 2 ? tool.Substring(0, tool.Length - 2).Trim() : String.Empty;
                            string unit = line.Substring(0, line.IndexOf("(", StringComparison.Ordinal));
                            resource.FullName = unit;
                            resource.RequestedEquipment.Add(tool);
                            operation.Resources.Add(resource);
                            
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
            DStraße,
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
