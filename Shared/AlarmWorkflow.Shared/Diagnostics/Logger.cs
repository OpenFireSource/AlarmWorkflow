using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using AlarmWorkflow.Shared.Core;

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

        private static readonly string DefaultLogPath = Utilities.GetLocalAppDataFolderPath();
        /// <summary>
        /// The maximum length of the full type name until it gets truncated at the beginning.
        /// This only applies if the source is inferred from the type name.
        /// </summary>
        private const int MaxSourceLength = 32;
        private const string LogExtension = "csv";

        #endregion

        #region Fields

        private int _cacheSize = 0;
        private List<LogEntry> _cacheCurrent;
        private List<ILoggingListener> _listeners;

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether or not cache mode is active.
        /// </summary>
        public bool IsCacheMode
        {
            get { return _cacheSize > 0; }
        }
        /// <summary>
        /// Gets the file of the output file where the logs are stored.
        /// This file must be absolute.
        /// </summary>
        public string OutputFileName { get; private set; }
        /// <summary>
        /// Gets/sets whether or not the logging of messages classified as "Debug" shall be logged, even if not compiled in DEBUG mode.
        /// </summary>
        public bool ForceDebugMessageLogging { get; set; }
        /// <summary>
        /// Gets/sets the max file size of the log file before it gets cleaned up.
        /// </summary>
        public int MaxFileSize { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the Logging class.
        /// </summary>
        private Logger()
        {
            // 512 KB are enough
            MaxFileSize = (int)(0.5f * 1024.0f * 1024.0f);

            _cacheCurrent = new List<LogEntry>(_cacheSize);

            _listeners = new List<ILoggingListener>();

            SetOutputFileName(Path.Combine(DefaultLogPath, "Log." + LogExtension));
        }

        #endregion

        #region Methods

        private void WriteToFile(params string[] textLines)
        {
            try
            {
                using (FileStream stream = new FileStream(OutputFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter strmWrite = new StreamWriter(stream))
                    {
                        foreach (string line in textLines)
                        {
                            strmWrite.WriteLine(line);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                string message = string.Format("Could not write to the log file. This is usually caused by a locked logfile. Do you have the log file opened in another program? The error message was: {0}", ex.Message);
                Trace.WriteLine(message);
            }
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
            bool isConsole = (entry.MessageType == LogType.Console);
            bool isNothing = (entry.MessageType == LogType.None);

#if !DEBUG
            // ignore designated DEBUG-only messages (they might contain information that the user does not necessarily need to know)
            // however this does not apply if the user wants these messages to be logged (this might be due to a debug switch argument)
            if (entry.MessageType == LoggingMessageType.Debug && !ForceDebugMessageLogging)
            {
                return;
            }
#endif
            // is logging to output file enabled?
            // this is only allowed when this is no console message
            if (!isConsole)
            {
                // To Cache, or not To Cache, that's the question
                if ((_cacheSize > 0)
                    && (_cacheCurrent.Count <= _cacheSize))
                {
                    _cacheCurrent.Add(entry);
                }
                else
                {
                    _cacheCurrent.Add(entry);
                    Flush();
                }
            }

            if (!isNothing)
            {
                // update our listeners
                // this goes asynchronous
                ThreadPool.QueueUserWorkItem(o =>
                {
                    // Create a copy of the listeners to avoid having a lock
                    var copy = _listeners.ToArray();

                    foreach (ILoggingListener listener in copy)
                    {
                        try
                        {
                            listener.Write(entry);
                        }
                        catch (Exception ex)
                        {
                            // Some listeners may not be robust enough. Trace information about the culprit.
                            // Then remove him from the list to avoid more errors.
                            _listeners.Remove(listener);

                            LogFormat(LogType.Error, this, "The log listener with type '{0}' caused an exception while writing. The logger will be disabled. Please see the log file.", listener.GetType().Name);
                            LogException(this, ex);
                        }
                    }
                });
            }
        }

        private void Flush()
        {
            lock (_cacheCurrent)
            {
                // check for max file size
                if (File.Exists(OutputFileName))
                {
                    FileInfo fi = new FileInfo(OutputFileName);
                    if (fi.Length >= MaxFileSize)
                    {
                        try
                        {
                            fi.Delete();
                        }
                        catch (Exception ex)
                        {
                            // silently catch exceptions in here
                            Trace.WriteLine("An error occurred while trying to write to the log file! The error message was: " + ex.Message);
                        }
                    }
                }

                List<string> entries = new List<string>(_cacheCurrent.Count);
                // alright, whe have cached it up long enough, now write it
                while (_cacheCurrent.Count > 0)
                {
                    entries.Add(_cacheCurrent[0].ToString());
                    _cacheCurrent.RemoveAt(0);
                }
                WriteToFile(entries.ToArray());
                _cacheCurrent.Clear();
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
        /// Registers a new <see cref="ILoggingListener"/>.
        /// </summary>
        /// <param name="listener">The <see cref="ILoggingListener"/> to register.</param>
        public void RegisterListener(ILoggingListener listener)
        {
            lock (_listeners)
            {
                _listeners.Add(listener);
            }
        }

        /// <summary>
        /// Unregisters an existing <see cref="ILoggingListener"/>.
        /// </summary>
        /// <param name="listener">The <see cref="ILoggingListener"/> to unregister.</param>
        public void UnregisterListener(ILoggingListener listener)
        {
            lock (_listeners)
            {
                _listeners.Remove(listener);
            }
        }

        /// <summary>
        /// Sets the caching mode for this logging instance.
        /// </summary>
        /// <remarks>Setting <paramref name="cacheSize"/> to a higher value results in less writes to the hard disk, while app crashes may result in an incomplete log file.
        /// Setting <paramref name="cacheSize"/> to a lower value results in more writes to the hard disk but ensures that the log is complete.</remarks>
        /// <param name="cacheSize">The cache size to set. See documentation for further information.</param>
        /// <exception cref="System.ArgumentException"><paramref name="cacheSize"/> was set to an invalid value (below zero or above an implementation-specific limit).</exception>
        public void SetCacheMode(int cacheSize)
        {
            Flush();
            _cacheSize = cacheSize;
        }

        /// <summary>
        /// Sets the output file.
        /// Next time something is logged it goes into the file given here.
        /// </summary>
        /// <param name="fileName">The new output file.</param>
        /// <exception cref="System.ArgumentException">Parameter <paramref name="fileName"/> was a file that was not absolute.</exception>
        public void SetOutputFileName(string fileName)
        {
            if (!Path.IsPathRooted(fileName))
            {
                throw new ArgumentException(Properties.Resources.FileNameMustBeAbsolute, "fileName");
            }
            else
            {
                string dir = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                OutputFileName = fileName;
            }
        }

        /// <summary>
        /// Initializes the logger.
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// Called when the host application is shutting down. Used to clean-up the caches (if any), finish writing to the hard disk, etc.
        /// </summary>
        public void Shutdown()
        {
            Flush();

            // shutdown and detach all listeners that are still alive
            lock (_listeners)
            {
                while (_listeners.Count > 0)
                {
                    // shut down this listener and remove it
                    _listeners[0].Shutdown();
                    _listeners.RemoveAt(0);
                }
            }
        }

        #endregion
    }
}