// <copyright file="AlarmworkflowService.cs" company="OpenFireSource.de">
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
// <date>2009-03-08</date>
// <summary>This File is the Startpoint for the AlarmworkflowService.</summary>

namespace OpenFireSource.Alarmworkflow.AlarmworkflowService
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Diagnostics;
	using System.ServiceProcess;
	using System.Text;
	using AlarmworkflowCodeLib;
	
	/// <summary>
	/// This class implements a Windows Service for the Alarmworkflow Projekt.
	/// </summary>
	public class AlarmworkflowServiceClass : ServiceBase
	{
		/// <summary>
		/// The Name of this Service.
		/// </summary>
		public const string MyServiceName = "AlarmworkflowService";
		
		/// <summary>
		/// An instance of the AlarmworkflowClass.
		/// </summary>
		private AlarmworkflowClass alarmWorkflow;
				
		/// <summary>
		/// Initializes a new instance of the AlarmworkflowServiceClass class.
		/// </summary>
		public AlarmworkflowServiceClass()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">Indicates the disposing state.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.alarmWorkflow = null;
			}
			
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// Start this service.
		/// </summary>
		/// <param name="args">Start Arguments. Not used in this class.</param>
		protected override void OnStart(string[] args)
		{
			try
			{
				this.alarmWorkflow = new AlarmworkflowClass();
				this.alarmWorkflow.Start();
			}
			catch (Exception ex)
			{
				EventLog eventLog1 = new EventLog("Application", ".");
				if (!System.Diagnostics.EventLog.SourceExists("AlarmWorkflow"))
				{
					System.Diagnostics.EventLog.CreateEventSource("AlarmWorkflow", "Application");
				}
				
				eventLog1.Source = "AlarmWorkflow";
				eventLog1.WriteEntry("Error while starting the alarmworkflow service: " + ex.ToString(), EventLogEntryType.Error);	
			}
		}
		
		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			this.alarmWorkflow.Stop();
		}
		
		/// <summary>
		/// Initialize all components.
		/// </summary>
		private void InitializeComponent()
		{
			this.ServiceName = MyServiceName;
		}
	}
}
