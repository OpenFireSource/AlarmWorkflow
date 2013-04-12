using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using AlarmWorkflow.Shared.Core;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Util;

namespace AlarmWorkflow.Shared.Diagnostics
{
    /// <summary>
    /// Represents the main logging facility.
    /// </summary>
    public sealed class Logger
    {
        #region Singleton

        private static Logger _instance;
        /// <summary>
        /// Gets the singleton <see cref="T:Logger"/> instance.
        /// </summary>
        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Logger();
                }
                return _instance;
            }
        }

        #endregion

        #region Constants

        private static readonly string DefaultLogPath = Path.Combine(Utilities.GetLocalAppDataFolderPath(), "Logs");

        #endregion

        #region Fields

        private ILog _log;
        private object _lock = new object();
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the Logging class.
        /// </summary>
        private Logger()
        {

        }

        #endregion

        #region Methods

        private static string GetLogDirectory(string logName)
        {
            return Path.Combine(DefaultLogPath, logName);
        }

        private string GetLogSourceName(object source)
        {
            if (source == null)
            {
                return "(Unknown source)";
            }
            else if (source is Type)
            {
                return ((Type)source).Name;
            }
            else if (source is string)
            {
                return (string)source;
            }

            return source.GetType().Name;
        }

        private void LogCore(LogEntry entry)
        {
            if (_log == null)
            {
                return;
            }
            ILogger logger = _log.Logger;

            LoggingEventData data = new LoggingEventData();
            data.TimeStamp = entry.Timestamp;
            data.Message = entry.Message;
            data.Level = GetLog4netLevel(entry.MessageType);
            data.LoggerName = _log.Logger.Name;

            if (entry.Exception != null)
            {
                data.ExceptionString = entry.Exception.ToString();
            }
            data.ThreadName = entry.Source;
            data.Properties = new PropertiesDictionary();
            data.Properties["Source"] = entry.Source;
            LoggingEvent le = new LoggingEvent(data);

            logger.Log(le);

        }

        private Level GetLog4netLevel(LogType logType)
        {
            switch (logType)
            {
                case LogType.Debug:
                    return Level.Debug;
                case LogType.Trace:
                    return Level.Fine;
                case LogType.Info:
                    return Level.Info;
                case LogType.Warning:
                    return Level.Warn;
                case LogType.Error:
                    return Level.Error;
                case LogType.Exception:
                    return Level.Fatal;
                case LogType.Console:
                case LogType.None:
                default:
                    return Level.Verbose;
            }
        }

        /// <summary>
        /// Logs a <see cref="Exception"/>.
        /// </summary>
        /// <param name="source">The component from which this type comes. The type name of the instance is used.</param>
        /// <param name="exception">The exception.</param>
        public void LogException(object source, Exception exception)
        {
            LogEntry entry = new LogEntry(LogType.Exception, GetLogSourceName(source), exception.Message);
            entry.Exception = exception;
            LogCore(entry);
        }

        /// <summary>
        /// Logs formatted text.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <param name="source">The component from which this type comes. The type name of the instance is used.</param>
        /// <param name="format">The text to use as the format string.</param>
        /// <param name="arguments">The arguments to use for the format string.</param>
        public void LogFormat(LogType type, object source, string format, params object[] arguments)
        {
            LogCore(new LogEntry(type, GetLogSourceName(source), String.Format(CultureInfo.InvariantCulture, format, arguments)));
        }

        /// <summary>
        /// Logs formatted text.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <param name="source">The component from which this type comes. The type name of the instance is used.</param>
        /// <param name="format">The text to use as the format string.</param>
        /// <param name="exception">The exception to log. If this is not null, it will create a separate entry just as "LogException" does.</param>
        /// <param name="arguments">The arguments to use for the format string.</param>
        public void LogFormat(LogType type, object source, string format, Exception exception, params object[] arguments)
        {
            LogEntry entry = new LogEntry(type, GetLogSourceName(source), String.Format(CultureInfo.InvariantCulture, format, arguments));
            entry.Exception = exception;
            LogCore(entry);
        }

        /// <summary>
        /// Initializes the logger.
        /// </summary>
        /// <param name="logName">The name of the log. This will be used as the folder name of this log.</param>
        /// <exception cref="System.InvalidOperationException">This instance is already initialized.</exception>
        public void Initialize(string logName)
        {
            Assertions.AssertNotEmpty(logName, "logName");

            if (_log != null)
            {
                throw new InvalidOperationException("This instance is already initialized!");
            }

            Log4netConfigurator.Configure(logName);

            _log = LogManager.GetLogger(logName);
        }

        #endregion

        #region Nested types

        private class RemoteLogger : RemotingAppender
        {
            protected override void Append(LoggingEvent loggingEvent)
            {
                LoggingEventData data = loggingEvent.GetLoggingEventData(FixFlags.All);
                data.ThreadName = ((String)loggingEvent.Properties["Source"]);
                LoggingEvent logging = new LoggingEvent(data);
                base.Append(logging);
            }
        }

        class Log4netConfigurator
        {
            internal static void Configure(string logName)
            {
                List<IAppender> appenders = new List<IAppender>();
                appenders.Add(CreateConsoleAppender(logName));
                appenders.Add(CreateTraceAppender(logName));
                appenders.Add(CreateFileAppender(logName));
                appenders.Add(CreateEventLogAppender(logName));
                appenders.Add(CreateRemoteAppender(logName));
                foreach (IOptionHandler handler in appenders.OfType<IOptionHandler>())
                {
                    handler.ActivateOptions();
                }

                log4net.Config.BasicConfigurator.Configure(appenders.ToArray());
            }

            private static IAppender CreateConsoleAppender(string logName)
            {
                ColoredConsoleAppender appender = new ColoredConsoleAppender();
                appender.AddMapping(new ColoredConsoleAppender.LevelColors() { Level = Level.Debug, ForeColor = ColoredConsoleAppender.Colors.White });
                appender.AddMapping(new ColoredConsoleAppender.LevelColors() { Level = Level.Info, ForeColor = ColoredConsoleAppender.Colors.White });
                appender.AddMapping(new ColoredConsoleAppender.LevelColors() { Level = Level.Fine, ForeColor = ColoredConsoleAppender.Colors.White });
                appender.AddMapping(new ColoredConsoleAppender.LevelColors() { Level = Level.Trace, ForeColor = ColoredConsoleAppender.Colors.White });
                appender.AddMapping(new ColoredConsoleAppender.LevelColors() { Level = Level.Warn, ForeColor = ColoredConsoleAppender.Colors.Yellow });
                appender.AddMapping(new ColoredConsoleAppender.LevelColors() { Level = Level.Error, ForeColor = ColoredConsoleAppender.Colors.Red });
                appender.AddMapping(new ColoredConsoleAppender.LevelColors() { Level = Level.Fatal, ForeColor = ColoredConsoleAppender.Colors.Red });
                appender.AddMapping(new ColoredConsoleAppender.LevelColors() { Level = Level.Critical, ForeColor = ColoredConsoleAppender.Colors.Red });
                appender.Layout = CreateOnlyMessageLayout();

                return appender;
            }
            private static IAppender CreateRemoteAppender(string logname)
            {
                RemoteLogger appender = new RemoteLogger();
                appender.Fix = FixFlags.All;
                appender.Evaluator = new TimeEvaluator(120);
                appender.Name = "AWF";
                appender.Sink = "tcp://localhost:9090/LoggingSink";
                appender.Lossy = false;
                appender.BufferSize = 1;
                return appender;
            }
            private static IAppender CreateTraceAppender(string logName)
            {
                TraceAppender appender = new TraceAppender();
                appender.Layout = CreateTraceLayout();
                return appender;
            }

            private static IAppender CreateFileAppender(string logName)
            {
                string logDirectory = Logger.GetLogDirectory(logName);
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                string logFileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".log";

                RollingFileAppender appender = new RollingFileAppender();
                appender.File = Path.Combine(logDirectory, logFileName);
                appender.Layout = CreateFileLayout();

                return appender;
            }

            private static IAppender CreateEventLogAppender(string logName)
            {
                EventLogAppender appender = new EventLogAppender();
                appender.Layout = CreateOnlyMessageLayout();
                appender.ApplicationName = "AlarmWorkflow/" + logName;

                return appender;
            }

            private static ILayout CreateTraceLayout()
            {
                PatternLayout layout = new PatternLayout(PatternLayout.DetailConversionPattern);

                layout.ActivateOptions();
                return layout;
            }

            private static ILayout CreateOnlyMessageLayout()
            {
                PatternLayout layout = new PatternLayout(PatternLayout.DefaultConversionPattern);

                layout.ActivateOptions();
                return layout;
            }

            private static ILayout CreateFileLayout()
            {
                XmlLayoutSchemaLog4j layoutSchemaLog4J = new XmlLayoutSchemaLog4j();
                layoutSchemaLog4J.ActivateOptions();
                return layoutSchemaLog4J;
            }
        }

        #endregion
    }
}