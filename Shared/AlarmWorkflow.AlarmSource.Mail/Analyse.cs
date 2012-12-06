#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace AlarmWorkflow.AlarmSource.Mail
{
    public class Analyse
    {
        private static readonly Dictionary<string, List<String>> einsatz = new Dictionary<string, List<string>>();
        private static string last = "";

        public static IDictionary<string, string> AnalyseData(String[] lines, IList<string> fields,
                                                              String fieldDemiliter, String multiLineJoin)
        {
            einsatz.Clear();
            if (lines == null) throw new ArgumentNullException("lines");
            if (fields == null) throw new ArgumentNullException("fields");
            if (fieldDemiliter == null) throw new ArgumentNullException("fieldDemiliter");
            if (multiLineJoin == null) throw new ArgumentNullException("multiLineJoin");
            foreach (string line in lines)
            {
                string current = line;
                current = current.Replace("—", "-");
                //Schaut wie viel Felder in der Zeile sind
                int z = CountStringOccurrences(current, fieldDemiliter);
                // Wenn ein Feld gefunden oder vorher bereits eins gefunden (Verwirft alle Felder vor dem 1. Feld)
                if (z != 0 || !String.IsNullOrWhiteSpace(last))
                {
                    //Wenn kein Feld gefunden und davor schon eins (erweitert das Feld davor (MULTILINE))
                    if (z == 0 && !String.IsNullOrWhiteSpace(last))
                    {
                        einsatz[last].Add(current);
                    }
                    //Ein Feld gefunden
                    if (z == 1)
                    {
                        //Index des Demiliters
                        int index = current.IndexOf(fieldDemiliter, StringComparison.Ordinal);
                        //String bis zum Demiliter
                        string key = current.Substring(0, index).Trim(' ');
                        //Prüft ob das gefunde Feld ein erlaubts
                        String foundField = "";
                        foreach (string field in fields.Where(field => key.ToLower().StartsWith(field.ToLower())))
                        {
                            foundField = field;
                            break;
                        }
                        //Wenn was gefunden (Feld is "zulässig")
                        if (foundField != "")
                        {
                            //Werte nach Demiliters
                            string value = current.Substring(index + 1);
                            //Strings 'verschönern'
                            value = value.Trim(' ');
                            key = key.Trim(' ');
                            einsatz.Add(key, new List<string> {value});
                            last = key;
                        }
                            //Wenn nix gefunden
                        else
                        {
                            //Wenn davor schon ein Feld gefunden
                            if (!String.IsNullOrWhiteSpace(last))
                            {
                                //Liste von vorheriger Zeile um die aktuelle Zeile erweitern
                                einsatz[last].Add(current);
                            }
                        }
                    }
                    //Zwei oder mehr Felder gefunden
                    if (z >= 2)
                    {
                        for (int i = 0; i < z; i++)
                        {
                            int index = current.IndexOf(fieldDemiliter, StringComparison.Ordinal);
                            string key = current.Substring(0, index).Trim(' ');
                            string rest = current.Substring(index + 1).Trim(' ');
                            string value = "";
                            bool test = rest.Contains(fieldDemiliter);
                            if (test)
                            {
                                int nextKeyEnd = rest.IndexOf(fieldDemiliter, StringComparison.Ordinal);
                                int nextKeyBeginn = rest.Substring(0, nextKeyEnd).LastIndexOf(" ", StringComparison
                                                                                                       .Ordinal);
                                if (nextKeyBeginn >= 0)
                                {
                                    value = rest.Substring(0, nextKeyBeginn);
                                    current = rest.Substring(nextKeyBeginn);
                                }
                                else
                                {
                                    value = rest.Trim(' ');
                                }
                            }
                            else
                            {
                                value = rest.Trim(' ');
                            }
                            String foundField = "";
                            foreach (string field in fields.Where(field => key.ToLower().StartsWith(field.ToLower())))
                            {
                                foundField = field;
                                break;
                            }
                            if (foundField != "")
                            {
                                einsatz.Add(foundField, new List<string> {value});
                                last = foundField;
                            }
                            else
                            {
                                if (!String.IsNullOrWhiteSpace(last))
                                {
                                    if (!einsatz[last].Contains(key + fieldDemiliter + value))
                                    einsatz[last].Add(key + fieldDemiliter + value);
                                }
                            }
                        }
                    }
                }
            }

            IDictionary<String, String> dictionary = einsatz.ToDictionary(entry => entry.Key,
                                                                          entry =>
                                                                          String.Join(multiLineJoin, entry.Value));
            return dictionary;
        }

        /// <summary>
        ///     Zählt Vorkommen eines Strings in einem anderen String
        /// </summary>
        public static int CountStringOccurrences(string text, string pattern)
        {
            // Loop through all instances of the string 'text'.
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i, StringComparison.Ordinal)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }

        public Dictionary<string, List<String>> getEinsatz()
        {
            return einsatz;
        }
    }
}