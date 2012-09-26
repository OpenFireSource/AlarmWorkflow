// <copyright file="MailingJob.cs" company="OpenFireSource.de">
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
// <summary>In this file the MailingJob class is immplemented.</summary>

namespace OpenFireSource.Alarmworkflow.AlarmworkflowCodeLib.Jobs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Net.Mail;
	using System.Text;
	using System.Xml;
	using System.Xml.XPath;
	
	/// <summary>
	/// Implements a Job, that send emails with all the operation information.
	/// </summary>
	public class MailingJob : IJob
	{
		#region private member
		/// <summary>
		/// The errormsg, if an error occured.
		/// </summary>
		private string errormsg;
		
		/// <summary>
		/// URL of the SMTP server.
		/// </summary>
		private string server;
		
		/// <summary>
		/// Sender email address.
		/// </summary>
		private string fromEmail;
		
		/// <summary>
		/// Username of the SMTP server.
		/// </summary>
		private string user;
		
		/// <summary>
		/// Password of the SMTP server.
		/// </summary>
		private string pwd;
		
		/// <summary>
		/// Stores all the Emails.
		/// </summary>
		private List<string> emaillist;
		#endregion

		#region constructors
		/// <summary>
		/// Initializes a new instance of the MailingJob class.
		/// </summary>
		/// <param name="settings">Xml node that has all the information to send emails.</param>
		/// <param name="debug">If true, emails will only send to debuging addresses.</param>
		public MailingJob(IXPathNavigable settings, bool debug)
		{
			this.emaillist = new List<string>();
			XPathNavigator nav = settings.CreateNavigator();
			if (nav.UnderlyingObject is XmlElement)
			{
				this.server = nav.SelectSingleNode("MailServer").InnerXml;
				this.fromEmail = nav.SelectSingleNode("FromMail").InnerXml;
				this.user = nav.SelectSingleNode("User").InnerXml;
				this.pwd = nav.SelectSingleNode("Pwd").InnerXml;
				XmlNode emailnode = ((XmlElement)nav.UnderlyingObject).SelectSingleNode("MailAdresses");
				XmlNodeList emails = emailnode.SelectNodes("MailAddress");
				for (int i = 0; i < emails.Count; i++)
				{
					if (debug == true)
					{
						XmlAttribute atr = emails.Item(i).Attributes["debug"];
						if (atr != null)
						{
							if (atr.InnerText.ToUpperInvariant() == "TRUE")
							{
								this.emaillist.Add(emails.Item(i).InnerText);
							}
						}
					}
					else
					{
						this.emaillist.Add(emails.Item(i).InnerText);
					}
				}
			}
			else
			{
				throw new ArgumentException("Settings is not an XmlElement");
			}
			
			this.errormsg = string.Empty;
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
		/// Inherited by iJob interface. Sends Emails to the list of addresse, with all of the operation information.
		/// </summary>
		/// <param name="einsatz">Current operation.</param>
		/// <returns>False when an error occured, otherwise true.</returns>
		public bool DoJob(Einsatz einsatz)
		{
			this.errormsg = string.Empty;
			SmtpClient client = new SmtpClient(this.server);
			
			// create the Mail
			System.Net.Mail.MailMessage message = new MailMessage();
			message.From = new MailAddress(this.fromEmail);
			foreach (string ma in this.emaillist)
			{
				message.To.Add(ma);
			}
			
			message.Subject = "FFWPlanegg Einsatz";
			message.Body += "Einsatznr: " + einsatz.Einsatznr + "\n";
			message.Body += "Mitteiler: " + einsatz.Mitteiler + "\n";
			message.Body += "Einsatzort: " + einsatz.Einsatzort + "\n";
			message.Body += "Strasse: " + einsatz.Strasse + "\n";
			message.Body += "Kreuzung: " + einsatz.Kreuzung + "\n";
			message.Body += "Ort: " + einsatz.Ort + "\n";
			message.Body += "Objekt: " + einsatz.Objekt + "\n";
			message.Body += "Meldebild: " + einsatz.Meldebild + "\n";
			message.Body += "Hinweis: " + einsatz.Hinweis + "\n";
			message.Body += "Einsatzplan: " + einsatz.Einsatzplan + "\n";
			
			message.BodyEncoding = Encoding.UTF8;
			
			// Authentifizierung
			NetworkCredential credential = new NetworkCredential(this.user, this.pwd);
			client.Credentials = credential;
			
			// send
			try
			{
				client.Send(message);
			}
			catch (ArgumentNullException e)
			{
				this.errormsg = e.ToString();
				return false;
			}
			catch (ArgumentOutOfRangeException e)
			{
				this.errormsg = e.ToString();
				return false;
			}
			catch (InvalidOperationException e)
			{
				this.errormsg = e.ToString();
				return false;
			}
			catch (SmtpException e)
			{
				this.errormsg = e.ToString();
				return false;
			}
			
			return true;
		}
		#endregion
	}
}
