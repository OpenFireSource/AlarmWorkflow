// <copyright file="DisplayWakeUp.cs" company="OpenFireSource.de">
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
// <summary>In this file the DisplayWakeUp class is immplemented.</summary>

namespace OpenFireSource.Alarmworkflow.AlarmworkflowCodeLib.Jobs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Security;
	using System.Text;
	using System.Xml;
	using System.Xml.XPath;

	/// <summary>
	/// Implements a Job, that turn on an Display/Monitor which is connected to a PowerAdapter.
	/// </summary>
	public class DisplayWakeUp : IJob
	{
		#region private members
		/// <summary>
		/// The errormsg, if an error occured.
		/// </summary>
		private string errormsg;
		
		/// <summary>
		/// The IP of the PowerAdapter.
		/// </summary>
		private string ip = string.Empty;
		
		/// <summary>
		/// The password if some is needed.
		/// </summary>
		private string pwd = string.Empty;
		
		/// <summary>
		/// The user name if some is needed.
		/// </summary>
		private string user = string.Empty;
		
		/// <summary>
		/// The port for the PowerAdapter.
		/// </summary>
		private string port = string.Empty;
		#endregion

		#region constructors
		/// <summary>
		/// Initializes a new instance of the DisplayWakeUp class.
		/// </summary>
		/// <param name="settings">Xml node that has all the information to connect to the lan-power-adapter.</param>
		public DisplayWakeUp(IXPathNavigable settings)
		{
			this.errormsg = string.Empty;
			XPathNavigator nav = settings.CreateNavigator();
			if (nav.UnderlyingObject is XmlElement)
			{
				this.ip = ((XmlElement)nav.UnderlyingObject).Attributes["ip"].InnerText;
				this.user = ((XmlElement)nav.UnderlyingObject).Attributes["user"].InnerText;
				this.pwd = ((XmlElement)nav.UnderlyingObject).Attributes["pwd"].InnerText;
				this. port = ((XmlElement)nav.UnderlyingObject).Attributes["port"].InnerText;
			}
			else
			{
				throw new ArgumentException("Settings is not an XmlElement");
			}
		}
		#endregion

		#region iJob Member
		
		/// <summary>
		/// Gets the error message.
		/// </summary>
		/// <value>
		/// Gets the error message.
		/// </value>
		public string ErrorMessage
		{
			get
			{
				return this.errormsg;
			}
		}

		/// <summary>
		/// Inherited by iJob interface. Wakes up a display, which is connected to a lan-power-adapter.
		/// </summary>
		/// <param name="einsatz">Current operation.</param>
		/// <returns>False when an error occured, otherwise true.</returns>
		public bool DoJob(Einsatz einsatz)
		{
			StringBuilder builder = new StringBuilder();
			
			//// http://admin:TFTPowerControl@192.168.0.243:80/SWITCH.CGI?s1=1

			builder.Append("http://");
			if (string.IsNullOrEmpty(this.user) == false)
			{
				builder.Append(this.user);
				builder.Append(":");
				builder.Append(this.pwd);
				builder.Append("@");
			}
			
			builder.Append(this.ip);
			builder.Append(":");
			builder.Append(this.port);
			builder.Append("/SWITCH.CGI?s1=1");	
			try
			{
				HttpWebRequest msg = (HttpWebRequest)System.Net.WebRequest.Create(new Uri(builder.ToString()));
				msg.GetResponse();
			}
			catch (ArgumentNullException ex)
			{
				this.errormsg = ex.ToString();
				return false;
			}
			catch (WebException ex)
			{
				this.errormsg = ex.ToString();
				return false;
			}
			catch (NotSupportedException ex)
			{
				this.errormsg = ex.ToString();
				return false;
			}
			catch (ProtocolViolationException ex)
			{
				this.errormsg = ex.ToString();
				return false;
			}
			catch (InvalidOperationException ex)
			{
				this.errormsg = ex.ToString();
				return false;
			}
			catch (SecurityException ex)
			{
				this.errormsg = ex.ToString();
				return false;
			}
			
			return true;
		}

		#endregion
	}
}
