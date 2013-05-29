using System;
using log4net;
using log4net.Appender;
using log4net.Core;

namespace AlarmWorkflow.Windows.ServiceMonitor.Helper
{
    internal class RemoteSink : MarshalByRefObject, RemotingAppender.IRemoteLoggingSink
    {
        void RemotingAppender.IRemoteLoggingSink.LogEvents(LoggingEvent[] events)
        {
            foreach (LoggingEvent loggingEvent in events)
            {
                LogManager.GetLogger("ServiceMonitor").Logger.Log(loggingEvent);
            }
        }
    }
}