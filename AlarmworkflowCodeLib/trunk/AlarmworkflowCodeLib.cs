// <copyright file="AlarmworkflowCodeLib.cs" company="OpenFireSource.de">
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
// <summary>This File is the Startpoint for the AlarmworkflowCodeLib.</summary>

namespace OpenFireSource.Alarmworkflow.AlarmworkflowCodeLib
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Security.Permissions;
	using System.Text;
	using System.Threading;
	using System.Xml;
	using OpenFireSource.Alarmworkflow.AlarmworkflowCodeLib.AlarmfaxParser;
	using OpenFireSource.Alarmworkflow.AlarmworkflowCodeLib.Jobs;
	using OpenFireSource.Alarmworkflow.AlarmworkflowCodeLib.Logging;
	
	/// <summary>
	/// Description of AlarmworkflowCodeLib.
	/// </summary>
	public class AlarmworkflowClass
	{
		/// <summary>
		/// Selected Logger.
		/// </summary>
		private ILogger logger;
		
		/// <summary>
		/// The running Thread.
		/// </summary>
		private Thread workingThread;
		
		/// <summary>
		/// The object who is running in the Thread.
		/// </summary>
		private WorkingThread workingThreadInstance;
		
		/// <summary>
		/// Initializes a new instance of the AlarmworkflowClass class.
		/// Constructor is reading the XML File, and safe the Settings in to the WorkingThread Instance.
		/// </summary>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public AlarmworkflowClass()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\AlarmWorkflow.xml");

			// Initialising Logger
			XmlNode node = doc.GetElementsByTagName("Logging")[0];
			string loggerstr = node.SelectSingleNode("Logger").Attributes["type"].Value;
			switch (loggerstr.ToUpperInvariant())
			{
				case "EVENTLOG":
					{
						this.Logger = new EventLogLogger();
						this.Logger.Enabled = true;
					}
					
					break;
				case "NONE":
				default:
					{
						this.Logger = new NoLogger();
						this.Logger.Enabled = false;
					}
					
					break;
			}
			
			if (!this.Logger.InitLogging())
			{				
				throw new InvalidProgramException("Exepection occurred during Logger initiialization!");
			}

			this.Logger.WriteInformation("Starting Service");

			// Thread Einstellungen initiieren
			node = doc.GetElementsByTagName("Service")[0];
			string faxPath = node.SelectSingleNode("FaxPath").InnerText;
			string archievPath = node.SelectSingleNode("ArchievPath").InnerText;
			string analysisPath = node.SelectSingleNode("AnalysisPath").InnerText;
			string ocr = node.SelectSingleNode("OCRSoftware").Attributes["type"].InnerText;
			string ocrpath = node.SelectSingleNode("OCRSoftware").Attributes["path"].InnerText;
			string parser = node.SelectSingleNode("AlarmfaxParser").InnerText;

			this.WorkingThreadInstance = new WorkingThread();

			this.WorkingThreadInstance.FaxPath = faxPath;
			this.WorkingThreadInstance.ArchievPath = archievPath;
			this.WorkingThreadInstance.Logger = this.Logger;
			this.WorkingThreadInstance.AnalysisPath = analysisPath;
			if (ocr.ToUpperInvariant() == "TESSERACT")
			{
				this.WorkingThreadInstance.UseOCRSoftware = OcrSoftware.Tesseract;
			}
			else
			{
				this.WorkingThreadInstance.UseOCRSoftware = OcrSoftware.Cuneiform;
			}
			
			this.WorkingThreadInstance.OcrPath = ocrpath;

			// AktiveJobs Options
			XmlNodeList aktiveJobsList = doc.GetElementsByTagName("AktiveJobs");
			XmlNode aktiveJobs = aktiveJobsList[0];
			string debugAktiveString = aktiveJobs.Attributes["DebugMode"].InnerText;
			bool debugaktive = false;
			if (debugAktiveString.ToUpperInvariant() == "TRUE")
			{
				debugaktive = true;
			}
			
			bool databaseAktive = true;
			bool smsAktive = true;
			bool mailAktive = true;
			bool replaceAktive = true;
			bool displayWakeUpAktive = true;
			foreach (XmlNode xnode in aktiveJobs.ChildNodes)
			{
				switch (xnode.Name)
				{
					case "Database":
						if (xnode.Attributes["aktive"].InnerText == "true")
						{
							databaseAktive = true;
						}
						else
						{
							databaseAktive = false;
						}
						
						break;
					case "SMS":
						if (xnode.Attributes["aktive"].InnerText == "true")
						{
							smsAktive = true;
						}
						else
						{
							smsAktive = false;
						}
						
						break;
					case "Mailing":
						if (xnode.Attributes["aktive"].InnerText == "true")
						{
							mailAktive = true;
						}
						else
						{
							mailAktive = false;
						}
						
						break;
					case "Replacing":
						if (xnode.Attributes["aktive"].InnerText == "true")
						{
							replaceAktive = true;
						}
						else
						{
							replaceAktive = false;
						}
						
						break;
					case "DisplayWakeUp":
						if (xnode.Attributes["aktive"].InnerText == "true")
						{
							displayWakeUpAktive = true;
						}
						else
						{
							displayWakeUpAktive = false;
						}
						
						break;
					default:
						break;
				}
			}

			XmlNode jobsSettings = doc.GetElementsByTagName("Jobs")[0];

			if (databaseAktive)
			{
				DatabaseJob dbjob = new DatabaseJob(jobsSettings.SelectSingleNode("DataBase"));
				this.WorkingThreadInstance.Jobs.Add(dbjob);
			}
			
			if (displayWakeUpAktive)
			{
				XmlNodeList displays = doc.GetElementsByTagName("Display");
				foreach (XmlNode display in displays)
				{
					this.WorkingThreadInstance.Jobs.Add(new DisplayWakeUp(display));
				}
			}
			
			if (mailAktive)
			{
				MailingJob mailjob = new MailingJob(jobsSettings.SelectSingleNode("Mailing"), debugaktive);
				this.WorkingThreadInstance.Jobs.Add(mailjob);
			}

			if (smsAktive)
			{
				SmsJob smsjob = new SmsJob(jobsSettings.SelectSingleNode("SMS"), debugaktive);
				this.WorkingThreadInstance.Jobs.Add(smsjob);
			}

			List<ReplaceString> rplist = new List<ReplaceString>();
			if (replaceAktive)
			{		
				XmlNode replacingNode = doc.GetElementsByTagName("replacing")[0];
				foreach (XmlNode rpn in replacingNode.ChildNodes)
				{
					ReplaceString rps = new ReplaceString();
					rps.OldString = rpn.Attributes["old"].InnerText;
					rps.NewString = rpn.Attributes["new"].InnerText;
					rplist.Add(rps);
				}
				
				this.WorkingThreadInstance.ReplacingList = rplist;
			}
				
			switch (parser)
			{
				case "MucLandParser":
				default:
					{
						this.WorkingThreadInstance.Parser = new MucLandParser(this.Logger, rplist);
					}
					
					break;
			}
		}

		/// <summary>
		/// Gets or sets the workingThread object.
		/// </summary>
		/// <value>
		/// Gets or sets the workingThread object.
		/// </value>
		public Thread WorkerThread
		{
			get { return this.workingThread; }
			set { this.workingThread = value; }
		}
		
		/// <summary>
		/// Gets or sets the workingThreadInstance object.
		/// </summary>
		internal WorkingThread WorkingThreadInstance
		{
			get { return this.workingThreadInstance; }
			set { this.workingThreadInstance = value; }
		}

		/// <summary>
		/// Gets or sets the logger object.
		/// </summary>
		internal ILogger Logger
		{
			get { return this.logger; }
			set { this.logger = value; }
		}
		
		/// <summary>
		/// Starts the monitor thread, which is waiting for a new Alarm.
		/// </summary>
		public void Start()
		{
			this.WorkerThread = new Thread(new ThreadStart(this.WorkingThreadInstance.DoWork));
			this.WorkerThread.Start();
			this.Logger.WriteInformation("Started Service");
		}

		/// <summary>
		/// Stops the Thread.
		/// </summary>
		public void Stop()
		{
			this.Logger.WriteInformation("Stopping Service");
			if (this.WorkerThread != null)
			{
				this.WorkerThread.Abort();
			}
			
			this.Logger.WriteInformation("Stopped Service");
		}
	}
}
