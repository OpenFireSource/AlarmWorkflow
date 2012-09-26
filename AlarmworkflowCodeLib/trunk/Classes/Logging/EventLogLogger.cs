// <copyright file="EventLogLogger.cs" company="OpenFireSource.de">
//	This file is part of the Alarmworkflow Projekt.
//
//	Alarmworkflow is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Alarmworkflow is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with Alarmworkflow.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <author>Andreas Glunz</author>
// <email>andreas@openfiresource.de</email>
// <date>2009-02-21</date>
// <summary>In this file the EventLogLogger class is immplemented.</summary>

namespace OpenFireSource.Alarmworkflow.AlarmworkflowCodeLib.Logging
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;

	/// <summary>
	/// The EventLogLogger class log all events to the windows event log.
	/// </summary>
	public class EventLogLogger : ILogger, IDisposable
	{
		/// <summary>
		/// The Eventlog, in which the logger logs.
		/// </summary>
		private EventLog eventLog1 = new EventLog("Application", ".");

		/// <summary>
		/// Inherited by iLogger abstract class. Initializes the logger.
		/// </summary>
		/// <returns>False when an error occured, otherwise true.</returns>
		public override bool InitLogging()
		{
			if (Enabled)
			{
				try
				{
					if (!System.Diagnostics.EventLog.SourceExists("AlarmWorkflow"))
					{
						System.Diagnostics.EventLog.CreateEventSource("AlarmWorkflow", "Application");
					}
					
					this.eventLog1.Source = "AlarmWorkflow";
				}
				catch (System.Security.SecurityException)
				{
					Enabled = false;
					return false;
				}
				catch (ArgumentException)
				{
					Enabled = false;
					return false;
				}
				catch (InvalidOperationException)
				{
					Enabled = false;
					return false;
				}
				
				return true;
			}
			
			return false;
		}

		/// <summary>
		/// Inherited by iLogger abstract class. Write some information to the Log.
		/// </summary>
		/// <param name="info">The information which will be loged.</param>
		public override void WriteInformation(string info)
		{
			if (Enabled)
			{
				this.eventLog1.WriteEntry(info, EventLogEntryType.Information);
			}
		}

		/// <summary>
		/// Inherited by iLogger abstract class. Write some warning to the Log.
		/// </summary>
		/// <param name="warning">The warning which will be loged.</param>
		public override void WriteWarning(string warning)
		{
			if (Enabled)
			{
				this.eventLog1.WriteEntry(warning, EventLogEntryType.Warning);
			}
		}

		/// <summary>
		/// Inherited by iLogger abstract class. Write some error to the Log.
		/// </summary>
		/// <param name="errorMessage">The error which will be loged.</param>
		public override void WriteError(string errorMessage)
		{
			if (Enabled)
			{
				this.eventLog1.WriteEntry(errorMessage, EventLogEntryType.Error);
			}
		}
		
		/// <summary>
		/// Inherited by IDisposable interface.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		/// <summary>
		/// Clean the object.
		/// </summary>
		/// <param name="alsoManaged">Indicates if also managed code shoud be cleaned up.</param>
		protected virtual void Dispose(bool alsoManaged)
		{
			if (alsoManaged == true)
			{
				this.eventLog1.Dispose();
				this.eventLog1 = null;
			}
		}
	}
}
