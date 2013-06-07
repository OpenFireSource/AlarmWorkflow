﻿using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Extensibility;

namespace AlarmWorkflow.Parser.Library
{
    [Export("LSTKleveParser", typeof(IParser))]
    class LSTKleveParser : IParser
    {
        #region Fields

        private readonly string[] _keywords = new[]
            {
                "Einsatznr", "EArt", "Stichwort",
                "Diagnose", "Meldender", "Priorität", "Ort ",
                "Ortsteil", "Straße", "Kreuzung", "NRN", "ADAC", "Info", "Objektname", "Routenausgabe", "beteiligte Einsatzmittel:", "Ausdruck", "BMA-Nummer"
            };

        #endregion

        #region IParser Members

        Operation IParser.Parse(string[] lines)
        {
            Operation operation = new Operation();
            CurrentSection section = CurrentSection.AAnfang;
            lines = Utilities.Trim(lines);
            foreach (string line in lines)
            {
                string keyword;
                if (GetKeyword(line.TrimStart(), out keyword))
                {
                    switch (keyword)
                    {
                        case "Einsatznr": { section = CurrentSection.BEinsatznr; break; }
                        case "EArt": { section = CurrentSection.CEArt; break; }
                        case "Stichwort": { section = CurrentSection.DStichwort; break; }
                        case "Diagnose": { section = CurrentSection.EDiagnose; break; }
                        case "Meldender": { section = CurrentSection.FMeldender; break; }
                        case "Priorität": { section = CurrentSection.GPriorität; break; }
                        case "Ort ": { section = CurrentSection.HOrt; break; }
                        case "Ortsteil": { section = CurrentSection.IOrtsteil; break; }
                        case "Straße": { section = CurrentSection.JStraße; break; }
                        case "Kreuzung": { section = CurrentSection.KKreuzung; break; }
                        case "ADAC": { section = CurrentSection.MADAC; break; }
                        case "Info": { section = CurrentSection.NInfo; break; }
                        case "NRN": { section = CurrentSection.LNRN; break; }
                        case "Objektname": { section = CurrentSection.OObjektname; break; }
                        case "BMA-Nummer": { section = CurrentSection.SBMA; break; }
                        case "Routenausgabe": { section = CurrentSection.PRoutenausgabe; break; }
                        case "beteiligte Einsatzmittel:": { section = CurrentSection.QEinsatzmittel; break; }
                        case "Ausdruck": { section = CurrentSection.REnde; break; }
                    }
                }


                switch (section)
                {
                    case CurrentSection.AAnfang:
                        {
                            break;
                        }
                    case CurrentSection.BEinsatznr:
                        {
                            operation.OperationNumber = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.CEArt:
                        {
                            operation.Keywords.Keyword = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.DStichwort:
                        {
                            operation.Keywords.EmergencyKeyword = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.EDiagnose:
                        {
                            operation.Picture = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.FMeldender:
                        {
                            operation.Messenger = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.GPriorität:
                        {
                            operation.Priority = GetMessageText(line);
                            section = CurrentSection.AAnfang;
                            break;
                        }
                    case CurrentSection.HOrt:
                        {
                            operation.Einsatzort.City = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.IOrtsteil:
                        {
                            operation.Einsatzort.City += " - " + GetMessageText(line);
                            break;
                        }
                    case CurrentSection.JStraße:
                        {
                            string text = GetMessageText(line);
                            Match hausnummer = Regex.Match(text, @"[0-9]+");
                            if (hausnummer.Success)
                            {
                                operation.Einsatzort.StreetNumber = hausnummer.Value;
                                operation.Einsatzort.Street = text.Replace(hausnummer.Value, "");
                            }
                            else
                            {
                                operation.Einsatzort.Street = text;
                            }
                            break;
                        }
                    case CurrentSection.KKreuzung:
                        {
                            operation.Einsatzort.Intersection = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.LNRN:
                        {
                            operation.CustomData.Add("NRN", GetMessageText(line));
                            break;
                        }
                    case CurrentSection.MADAC:
                        {
                            operation.CustomData.Add("ADAC", GetMessageText(line));
                            break;
                        }
                    case CurrentSection.NInfo:
                        {
                            operation.Comment = GetMessageText(line);
                            break;
                        }
                    case CurrentSection.OObjektname:
                        {
                            operation.Einsatzort.Property = GetMessageText(line);
                            operation.Einsatzort.Property = operation.Einsatzort.Property.Trim();
                            break;
                        }
                    case CurrentSection.SBMA:
                        {
                            operation.Einsatzort.Property += " BMA: " + GetMessageText(line);
                            operation.Einsatzort.Property = operation.Einsatzort.Property.Trim();
                            break;
                        }
                    case CurrentSection.PRoutenausgabe:
                        {
                            //TODO: Auswerten wenn Format bekannt ist.
                            break;
                        }
                    case CurrentSection.QEinsatzmittel:
                        {
                            Match alarmtime = Regex.Match(line, @"(([01]?\d|2[0-3]):[0-5]\d:[0-5]\d)|(--:--:--)");
                            if (alarmtime.Success)
                            {
                                string time = alarmtime.Value;
                                string unit = line.Replace(time, "").Trim();
                                operation.Resources.Add(new OperationResource { FullName = unit, Timestamp = time });
                            }
                            break;
                        }
                    case CurrentSection.REnde:
                        {
                            Match datetime = Regex.Match(line, @"[123]\d\. \w* 20\d{2}, (([01]?\d|2[0-3]):[0-5]\d)");
                            if (datetime.Success)
                            {
                                CultureInfo ci = new CultureInfo("de");
                                DateTime timeStamp;
                                operation.Timestamp = DateTime.TryParse(datetime.Value, ci, DateTimeStyles.None, out timeStamp) ? timeStamp : DateTime.Now;

                            }
                            break;
                        }
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
        private string GetMessageText(string line, string prefix = null)
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
            BEinsatznr,
            CEArt,
            DStichwort,
            EDiagnose,
            FMeldender,
            GPriorität,
            HOrt,
            IOrtsteil,
            JStraße,
            KKreuzung,
            LNRN,
            MADAC,
            NInfo,
            OObjektname,
            PRoutenausgabe,
            QEinsatzmittel,
            REnde,
            SBMA
        }

        #endregion
    }
}