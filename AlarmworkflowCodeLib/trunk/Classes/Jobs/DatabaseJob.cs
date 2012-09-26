// <copyright file="DatabaseJob.cs" company="OpenFireSource.de">
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
// <summary>In this file the DatabaseJob class is immplemented.</summary>
namespace OpenFireSource.Alarmworkflow.AlarmworkflowCodeLib.Jobs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Xml;
	using System.Xml.XPath;
	using MySql.Data.MySqlClient;

	/// <summary>
	/// Implements a job, who saves all the operation data to a MySQL database.
	/// </summary>
	public class DatabaseJob : IJob
	{
		#region private members
		/// <summary>
		/// Saves the errormsg, if an error occured.
		/// </summary>
		private string errormsg;
		
		/// <summary>
		/// The database user.
		/// </summary>
		private string user;
		
		/// <summary>
		/// The database users passoword.
		/// </summary>
		private string pwd;
		
		/// <summary>
		/// Name of the database.
		/// </summary>
		private string database;
		
		/// <summary>
		/// URL o the MySQL server.
		/// </summary>
		private string server;
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the DatabaseJob class.
		/// </summary>
		/// <param name="settings">Xml node that has all the information to connect to the database.</param>
		public DatabaseJob(IXPathNavigable settings)
		{
			this.errormsg = string.Empty;
			XPathNavigator nav = settings.CreateNavigator();
			this.database = nav.SelectSingleNode("DBName").InnerXml;
			this.user = nav.SelectSingleNode("UserID").InnerXml;
			this.pwd = nav.SelectSingleNode("UserPWD").InnerXml;
			this.server = nav.SelectSingleNode("DBServer").InnerXml;
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
		/// Inherited by iJob interface. Saves the current operation to a MySQL database.
		/// </summary>
		/// <param name="einsatz">Current operation.</param>
		/// <returns>False when an error occured, otherwise true.</returns>
		public bool DoJob(Einsatz einsatz)
		{
			this.errormsg = string.Empty;
			try
			{
				MySqlConnection conn = new MySqlConnection("Persist Security Info=False;database=" + this.database + ";server=" + this.server + ";user id=" + this.user + ";Password=" + this.pwd);
				conn.Open();
				if (conn.State != System.Data.ConnectionState.Open)
				{
					this.errormsg = "Coud not open SQL Connection";
					return false;
				}
				
				string cmdText = "INSERT INTO tb_einstaz (Einsatznr, Einsatzort, Einsatzplan, Hinweis, Kreuzung, Meldebild, Mitteiler, Objekt, Ort, Strasse, Stichwort) VALUES ('" + einsatz.Einsatznr + "', '" + einsatz.Einsatzort + "', '" + einsatz.Einsatzplan + "', '" + einsatz.Hinweis + "', '" + einsatz.Kreuzung + "', '" + einsatz.Meldebild + "', '" + einsatz.Mitteiler + "', '" + einsatz.Objekt + "', '" + einsatz.Ort + "', '" + einsatz.Strasse + "', '" + einsatz.Stichwort + "')";
				MySqlCommand cmd = new MySqlCommand(cmdText, conn);
				cmd.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				this.errormsg = ex.ToString();
				return false;
			}
			
			return true;
		}
		
		#endregion		
	}
}
