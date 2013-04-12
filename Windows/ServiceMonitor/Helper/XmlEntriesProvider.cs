using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using log4net.Core;

namespace AlarmWorkflow.Windows.ServiceMonitor.Helper
{
    internal class XmlEntriesProvider
    {
        public static List<LoggingEvent> GetEntries(string dataSource)
        {
            XmlReaderSettings settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };
            NameTable nt = new NameTable();
            XmlNamespaceManager mgr = new XmlNamespaceManager(nt);
            mgr.AddNamespace("log4j", "http://jakarta.apache.org/log4j");
            XmlParserContext pc = new XmlParserContext(nt, mgr, string.Empty, XmlSpace.Default);
            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            List<LoggingEvent> events = new List<LoggingEvent>();
            using (FileStream stream = new FileStream(dataSource, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.Default, true))
                {
                    using (XmlReader xmlTextReader = XmlReader.Create(reader, settings, pc))
                    {
                        while (xmlTextReader.Read())
                        {
                            if ((xmlTextReader.NodeType != XmlNodeType.Element) || (xmlTextReader.Name != "log4j:event"))
                                continue;
                            LoggingEventData entryData = new LoggingEventData();
                            entryData.LoggerName = xmlTextReader.GetAttribute("logger");
                            entryData.TimeStamp = date.AddMilliseconds(Convert.ToDouble(xmlTextReader.GetAttribute("timestamp"))).ToLocalTime();
                            entryData.Level = GetLogLevel(xmlTextReader.GetAttribute("level"));
                            entryData.ThreadName = xmlTextReader.GetAttribute("thread");

                            while (xmlTextReader.Read())
                            {
                                bool breakLoop = false;
                                switch (xmlTextReader.Name)
                                {
                                    case "log4j:event":
                                        breakLoop = true;
                                        break;
                                    default:
                                        switch (xmlTextReader.Name)
                                        {
                                            case ("log4j:message"):
                                                entryData.Message = xmlTextReader.ReadString();
                                                break;
                                            case ("log4j:data"):
                                                switch (xmlTextReader.GetAttribute("name"))
                                                {
                                                    case ("log4net:UserName"):
                                                        entryData.UserName = xmlTextReader.GetAttribute("value");
                                                        break;
                                                    case ("log4japp"):
                                                        entryData.Identity = xmlTextReader.GetAttribute("value");
                                                        break;
                                                }
                                                break;
                                            case ("log4j:throwable"):
                                                entryData.ExceptionString = xmlTextReader.ReadString();
                                                break;
                                            case ("log4j:locationInfo"):
                                                string classname = xmlTextReader.GetAttribute("class");
                                                string methodname = xmlTextReader.GetAttribute("method");
                                                string filename = xmlTextReader.GetAttribute("file");
                                                string linenumber = xmlTextReader.GetAttribute("line");
                                                LocationInfo location = new LocationInfo(classname, methodname, filename, linenumber);

                                                entryData.LocationInfo = location;
                                                break;
                                        }
                                        break;
                                }
                                if (breakLoop) break;
                            }

                            LoggingEvent entry = new LoggingEvent(entryData);
                            events.Add(entry);
                        }
                    }
                }
            }
            return events;
        }

        private static Level GetLogLevel(string level)
        {
            string ul = !String.IsNullOrWhiteSpace(level) ? level.Trim().ToUpper() : string.Empty;
            switch (ul)
            {
                case "DEBUG":
                    return Level.Debug;
                case "INFO":
                    return Level.Info;
                case "FINE":
                    return Level.Fine;
                case "WARN":
                    return Level.Warn;
                case "ERROR":
                    return Level.Error;
                case "FATAL":
                    return Level.Fatal;
                default:
                    return Level.Verbose;
            }
        }
    }
}