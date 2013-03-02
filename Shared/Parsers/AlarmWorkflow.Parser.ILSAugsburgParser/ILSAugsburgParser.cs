﻿using System;
using System.Text.RegularExpressions;
using AlarmWorkflow.AlarmSource.Fax;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Parser.ILSAugsburgParser
{
    [Export("ILSAugsburgParser", typeof(IFaxParser))]
    public class ILSAugsburgParser : IFaxParser
    {
        #region Constants

        private static readonly string[] Keywords = new[] { "", "ABSENDER", "NAME", "STRAßE", "ORT", "OBJEKT", "KREUZUNG", "STICHWORT", "SCHLAGW" };

        #endregion

        #region Methods

        private DateTime ReadFaxTimestamp(string line, DateTime fallback)
        {
            DateTime date = fallback;
            TimeSpan timestamp = date.TimeOfDay;

            Match dt = Regex.Match(line, @"(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d");
            Match ts = Regex.Match(line, @"([01]?[0-9]|2[0-3]):[0-5][0-9]");
            if (dt.Success)
            {
                DateTime.TryParse(dt.Value, out date);
            }
            if (ts.Success)
            {
                TimeSpan.TryParse(ts.Value, out timestamp);
            }

            return new DateTime(date.Year, date.Month, date.Day, timestamp.Hours, timestamp.Minutes, timestamp.Seconds, timestamp.Milliseconds, DateTimeKind.Local);
        }

        private bool StartsWithKeyword(string line, out string keyword)
        {
            line = line.ToUpperInvariant();
            foreach (string kwd in Keywords)
            {
                if (line.StartsWith(kwd))
                {
                    keyword = kwd;
                    return true;
                }
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

        /// <summary>
        /// Attempts to read the zip code from the city, if available.
        /// </summary>
        /// <param name="cityText"></param>
        /// <returns>The zip code of the city. -or- null, if there was no.</returns>
        private string ReadZipCodeFromCity(string cityText)
        {
            string zipCode = "";
            foreach (char c in cityText)
            {
                if (char.IsNumber(c))
                {
                    zipCode += c;
                    continue;
                }
                break;
            }
            return zipCode;
        }
        private bool GetSection(String line, ref CurrentSection section, ref bool keywordsOnly)
        {           
            if (line.Contains("MITTEILER"))
            {
                section = CurrentSection.BMitteiler;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("EINSATZORT"))
            {
                section = CurrentSection.CEinsatzort;
                //Is not true but only works that way
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("ZIELORT"))
            {
                section = CurrentSection.DZielort;
                //Is not true but only works that way
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("EINSATZGRUND"))
            {
                section = CurrentSection.EEinsatzgrund;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("EINSATZMITTEL"))
            {
                section = CurrentSection.FEinsatzmittel;
                keywordsOnly = true;
                return true;
            }
            if (line.Contains("BEMERKUNG"))
            {
                section = CurrentSection.GBemerkung;
                keywordsOnly = false;
                return true;
            }
            if (line.Contains("ENDE FAX"))
            {
                section = CurrentSection.HFooter;
                keywordsOnly = false;
                return true;
            }
            return false;
        }
        #endregion

        #region IFaxParser Members

        Operation IFaxParser.Parse(string[] lines)
        {
            Operation operation = new Operation();
            OperationResource last = new OperationResource();

            lines = Utilities.Trim(lines);

            CurrentSection section = CurrentSection.AHeader;
            bool keywordsOnly = true;

            InnerSection innerSection = InnerSection.AStraße;
            for (int i = 0; i < lines.Length; i++)
            {
                try
                {
                    string line = lines[i];
                    if (line.Length == 0)
                    {
                        continue;
                    }

                    // Try to parse the header and extract date and time if possible
                    operation.Timestamp = ReadFaxTimestamp(line, operation.Timestamp);


                    if (GetSection(line.Trim(), ref section, ref keywordsOnly))
                    {
                        continue;
                    }

                    string msg = line;
                    string prefix = "";

                    // Make the keyword check - or not (depends on the section we are in; see above)
                    if (keywordsOnly)
                    {
                        string keyword;
                        if (!StartsWithKeyword(line, out keyword))
                        {
                            continue;
                        }

                        int x = line.IndexOf(':');
                        if (x == -1)
                        {
                            // If there is no colon found (may happen occasionally) then simply remove the length of the keyword from the beginning
                            prefix = keyword;
                            msg = line.Remove(0, prefix.Length).Trim();
                        }
                        else
                        {
                            prefix = line.Substring(0, x);
                            msg = line.Substring(x + 1).Trim();
                        }

                        prefix = prefix.Trim().ToUpperInvariant();
                    }

                    // Parse each section
                    switch (section)
                    {
                        case CurrentSection.AHeader:
                            //TODO: Absender unterbringen
                            break;
                        case CurrentSection.BMitteiler:
                            {
                                // This switch would not be necessary in this section (there is only "Name")...
                                switch (prefix)
                                {
                                    case "NAME":
                                        operation.Messenger = msg;
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.CEinsatzort:
                            {

                                switch (prefix)
                                {
                                    case "STRAßE":
                                        {
                                            innerSection = InnerSection.AStraße;
                                            // The street here is mangled together with the street number. Dissect them...
                                            int streetNumberColonIndex = msg.LastIndexOf(':');
                                            if (streetNumberColonIndex != -1)
                                            {
                                                // We need to check for occurrence of the colon, because it may have been omitted by the OCR-software
                                                string streetNumber = msg.Remove(0, streetNumberColonIndex + 1).Trim();
                                                operation.Einsatzort.StreetNumber = streetNumber;
                                            }

                                            operation.Einsatzort.Street = msg.Substring(0, msg.IndexOf("Haus-", StringComparison.Ordinal)).Trim();
                                        }
                                        break;
                                    case "ORT":
                                        {
                                            innerSection = InnerSection.BOrt;
                                            operation.Einsatzort.ZipCode = ReadZipCodeFromCity(msg);
                                            if (string.IsNullOrWhiteSpace(operation.Einsatzort.ZipCode))
                                            {
                                                Logger.Instance.LogFormat(LogType.Warning, this, "Could not find a zip code for city '{0}'. Route planning may fail or yield wrong results!", operation.Einsatzort.City);
                                            }

                                            operation.Einsatzort.City = msg.Remove(0, operation.Einsatzort.ZipCode.Length).Trim();

                                            // The City-text often contains a dash after which the administrative city appears multiple times (like "City A - City A City A").
                                            // However we can (at least with google maps) omit this information without problems!
                                            int dashIndex = operation.Einsatzort.City.IndexOf('-');
                                            if (dashIndex != -1)
                                            {
                                                // Ignore everything after the dash
                                                operation.Einsatzort.City = operation.Einsatzort.City.Substring(0, dashIndex).Trim();
                                            }
                                        }
                                        break;
                                    case "OBJEKT":
                                        innerSection = InnerSection.CObjekt;
                                        operation.Einsatzort.Property = msg;
                                        break;
                                    case "KREUZUNG":
                                        operation.Einsatzort.Intersection = msg;
                                        break;
                                    default:
                                        switch (innerSection)
                                        {
                                            case InnerSection.AStraße:
                                                //Quite dirty because of Streetnumber. Looking for better solution
                                                operation.Einsatzort.Street += msg;
                                                break;
                                            case InnerSection.BOrt:
                                                operation.Einsatzort.City += msg;
                                                break;
                                            case InnerSection.CObjekt:
                                                operation.Einsatzort.Property += msg;
                                                break;
                                        }
                                        break;
                                }
                            }
                            break;
                        case CurrentSection.DZielort:
                            switch (prefix)
                            {

                                case "STRAßE":
                                    {
                                        innerSection = InnerSection.AStraße;
                                        // The street here is mangled together with the street number. Dissect them...
                                        int streetNumberColonIndex = msg.LastIndexOf(':');
                                        if (streetNumberColonIndex != -1)
                                        {
                                            // We need to check for occurrence of the colon, because it may have been omitted by the OCR-software
                                            operation.Zielort.StreetNumber = msg.Remove(0, streetNumberColonIndex + 1).Trim();
                                        }

                                        operation.Zielort.Street = msg.Substring(0, msg.IndexOf("Haus-", StringComparison.Ordinal)).Trim();
                                    }
                                    break;
                                case "ORT":
                                    {
                                        innerSection = InnerSection.BOrt;
                                        operation.Zielort.ZipCode = ReadZipCodeFromCity(msg);
                                        if (string.IsNullOrWhiteSpace(operation.Zielort.ZipCode))
                                        {
                                            Logger.Instance.LogFormat(LogType.Warning, this, "Could not find a zip code for city '{0}'. Route planning may fail or yield wrong results!", operation.Zielort.City);
                                        }

                                        operation.Zielort.City = msg.Remove(0, operation.Zielort.ZipCode.Length).Trim();
                                    }
                                    break;
                                case "OBJEKT":
                                    innerSection = InnerSection.CObjekt;
                                    operation.Zielort.Property = msg;
                                    break;
                                default:
                                    switch (innerSection)
                                    {
                                        case InnerSection.AStraße:
                                            //Quite dirty because of Streetnumber. Looking for better solution
                                            operation.Einsatzort.Street += msg;
                                            break;
                                        case InnerSection.BOrt:
                                            operation.Einsatzort.City += msg;
                                            break;
                                        case InnerSection.CObjekt:
                                            operation.Einsatzort.Property += msg;
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case CurrentSection.EEinsatzgrund:
                            {
                                switch (prefix)
                                {
                                    case "SCHLAGW.":
                                        operation.Keywords.Keyword = msg;
                                        break;
                                    case "STICHWORT":
                                        operation.Keywords.EmergencyKeyword = msg;
                                        break;

                                }
                            }
                            break;
                        case CurrentSection.FEinsatzmittel:
                            {
                                operation.Resources.Add(new OperationResource { FullName = msg.Substring(0, msg.IndexOf('(')) });
                            }
                            break;
                        case CurrentSection.GBemerkung:
                            {
                                // Append with newline at the end in case that the message spans more than one line
                                operation.Comment = operation.Comment += msg + "\n";
                            }
                            break;
                        case CurrentSection.HFooter:
                            // The footer can be ignored completely.
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, "Error while parsing line '{0}'. The error message was: {1}", i, ex.Message);
                }
            }

            // Post-processing the operation if needed
            if (!string.IsNullOrWhiteSpace(operation.Comment) && operation.Comment.EndsWith("\n"))
            {
                operation.Comment = operation.Comment.Substring(0, operation.Comment.Length - 1).Trim();
            }
            return operation;
        }

        #endregion

        #region Nested types

        private enum CurrentSection
        {
            AHeader,
            BMitteiler,
            CEinsatzort,
            DZielort,
            EEinsatzgrund,
            FEinsatzmittel,
            GBemerkung,
            /// <summary>
            /// Footer text. Introduced by "ENDE FAX". Can be ignored completely.
            /// </summary>
            HFooter
        }
        private enum InnerSection
        {
            AStraße,
            BOrt,
            CObjekt,
        }
        #endregion
    }
}
