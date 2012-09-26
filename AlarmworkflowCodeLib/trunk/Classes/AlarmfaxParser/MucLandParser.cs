// <copyright file="MucLandParser.cs" company="OpenFireSource.de">
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
// <date>2009-03-17</date>
// <summary>This file implements the MucLandParser class.</summary>

namespace OpenFireSource.Alarmworkflow.AlarmworkflowCodeLib.AlarmfaxParser
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Threading;
	using OpenFireSource.Alarmworkflow.AlarmworkflowCodeLib.Logging;
	
	/// <summary>
	/// Description of MucLandParser.
	/// </summary>
	public class MucLandParser : IParser
	{	
		/// <summary>
		/// The Logger object.
		/// </summary>
		private ILogger logger;
		
		/// <summary>
		/// The replace string class.
		/// </summary>
		private List<ReplaceString> replaceList;
		
		/// <summary>
		/// Initializes a new instance of the MucLandParser class.
		/// </summary>
		/// <param name="logger">The logger object.</param>
		/// <param name="replaceList">The RreplaceList object.</param>
		public MucLandParser(ILogger logger, List<ReplaceString> replaceList)
		{
			this.logger = logger;
			this.replaceList = replaceList;
		}
		
		/// <summary>
		/// This Methode is parsing a ocr text file and fill an einsatz object.
		/// </summary>
		/// <param name="file">Full path to the ocr file.</param>
		/// <returns>A filled einsatz object.</returns>
		public Einsatz ParseEinsatz(string file)
		{
			Einsatz einsatz = new Einsatz();
			string line = string.Empty;
			bool fileNotFound = true;
			int trys = 0;
			while (fileNotFound)
			{
				fileNotFound = false;
				trys++;
				try
				{
					StreamReader reader = new StreamReader(file);
					while ((line = reader.ReadLine()) != null)
					{
						string msg;
						string prefix;
						int x = line.IndexOf(':');
						if (x != -1)
						{
							prefix = line.Substring(0, x);
							msg = line.Substring(x + 1).Trim();
							if (this.replaceList != null)
							{
								foreach (ReplaceString rps in this.replaceList)
								{
									msg = msg.Replace(rps.OldString, rps.NewString);
								}
							}
							
							prefix = prefix.Trim().ToUpperInvariant();
							switch (prefix)
							{
								case "EINSATZNR":
									einsatz.Einsatznr = msg;
									break;
								case "MITTEILER":
									einsatz.Mitteiler = msg;
									break;
								case "EINSATZORT":
									einsatz.Einsatzort = msg;
									break;
								case "STRAßE":
								case "STRABE":
									einsatz.Strasse = msg;
									break;
								case "KREUZUNG":
									einsatz.Kreuzung = msg;
									break;
								case "ORTSTEIL/ORT":
									einsatz.Ort = msg;
									break;
								case "OBJEKT":
								case "9BJEKT":
									einsatz.Objekt = msg;
									break;
								case "MELDEBILD":
									einsatz.Meldebild = msg;
									break;
								case "HINWEIS":
									einsatz.Hinweis = msg;
									break;
								case "EINSATZPLAN":
									einsatz.Einsatzplan = msg;
									break;
							}
						}
					}
					
					reader.Close();
				}
				catch (FileNotFoundException ex)
				{
					if (trys < 10)
					{
						fileNotFound = true;
						Thread.Sleep(1000);
						this.logger.WriteWarning("Ocr file not found. Try " + trys.ToString(CultureInfo.InvariantCulture) + " of 10!");
					}
					else
					{
						this.logger.WriteError("Ocr File not found! " + ex.ToString());
					}
				}
				catch (Exception ex)
				{
					this.logger.WriteError(ex.ToString());
				}
			}
			
			return einsatz;
		}
	}
}
