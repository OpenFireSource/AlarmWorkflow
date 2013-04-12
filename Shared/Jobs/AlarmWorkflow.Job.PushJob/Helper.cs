using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AlarmWorkflow.Job.PushJob
{
    class Helper
    {
        internal static string ParseResponse(string response)
        {
            string serverReturn = null;
            try
            {
                var resultDir = new Dictionary<string, string>();
                XDocument result = XDocument.Parse(response);
                if (result.Root != null)
                {
                    IEnumerable<XElement> test = result.Root.Elements();
                    foreach (XElement xElement in test)
                    {
                        if (xElement.HasAttributes)
                        {
                            foreach (XAttribute attribute in xElement.Attributes())
                            {
                                resultDir.Add(attribute.Name.ToString(), attribute.Value);
                            }
                        }
                        if (!String.IsNullOrWhiteSpace(xElement.Value))
                        {
                            resultDir.Add(xElement.Name.ToString(), xElement.Value);
                        }
                    }
                }
                 serverReturn = DictToString(resultDir, null);
            }
            catch (Exception exception)
            {
            }
            return serverReturn;
        }
        private static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        private static string DictToString(IEnumerable<KeyValuePair<String, String>> items, string format)
        {
            format = String.IsNullOrEmpty(format) ? "{0} was '{1}' " : format;

            var itemString = new StringBuilder();
            foreach (var item in items)
                itemString.AppendFormat(format, UppercaseFirst(item.Key), item.Value);

            return itemString.ToString();
        }
    }
}
