// <copyright file="Einsatz.cs" company="OpenFireSource.de">
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
// <summary>This File implements the Einsatz Object.</summary>
namespace OpenFireSource.Alarmworkflow.AlarmworkflowCodeLib
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	
	/// <summary>
	/// This Class wraps an Operation.
	/// </summary>
	public class Einsatz
	{
		/// <summary>
		/// This is the operation number.
		/// </summary>
		private string einsatznr;
		
		/// <summary>
		/// This is the operation caller.
		/// </summary>
		private string mitteiler;
		
		/// <summary>
		/// This is the operation place.
		/// </summary>
		private string einsatzort;
		
		/// <summary>
		/// This is the operation street.
		/// </summary>
		private string strasse;
		
		/// <summary>
		/// This is the operation crossing.
		/// </summary>
		private string kreuzung;
		
		/// <summary>
		/// This is the place/city.
		/// </summary>
		private string ort;
		
		/// <summary>
		/// This is the operation object.
		/// </summary>
		private string objekt;
		
		/// <summary>
		/// This is the operation (meldebild).
		/// </summary>
		private string meldebild;
		
		/// <summary>
		/// This is the operation hint.
		/// </summary>
		private string hinweis;
		
		/// <summary>
		/// This is the operation plan.
		/// </summary>
		private string einsatzplan;

		/// <value>
		/// Gets or sets the Einsatznr object.
		/// </value>
		/// <summary>
		/// Gets or sets the Einsatznr object.
		/// </summary>
		public string Einsatznr
		{
			get
			{
				if (this.einsatznr == null)
				{
					this.einsatznr = string.Empty;
				}
				
				return this.einsatznr;
			}
			
			set
			{
				this.einsatznr = value;
			}
		}

		/// <value>
		/// Gets or sets the Mitteiler object.
		/// </value>
		/// <summary>
		/// Gets or sets the Mitteiler object.
		/// </summary>
		public string Mitteiler
		{
			get
			{
				if (this.mitteiler == null)
				{
					this.mitteiler = string.Empty;
				}
				
				return this.mitteiler;
			}
			
			set
			{
				if (value.ToUpperInvariant().Contains("HAUPTMELDER"))
				{
					this.mitteiler = value.Substring(value.IndexOf(" ", StringComparison.OrdinalIgnoreCase) + 1);
				}
				else
				{
					this.mitteiler = value;
				}
			}
		}

		/// <value>
		/// Gets or sets the Einsatzort object.
		/// </value>
		/// <summary>
		/// Gets or sets the Einsatzort object.
		/// </summary>
		public string Einsatzort
		{
			get
			{
				if (this.einsatzort == null)
				{
					this.einsatzort = string.Empty;
				}
				
				return this.einsatzort;
			}
			
			set
			{
				this.einsatzort = value;
			}
		}
		
		/// <value>
		/// Gets or sets the Strasse object.
		/// </value>
		/// <summary>
		/// Gets or sets the Strasse object.
		/// </summary>
		public string Strasse
		{
			get
			{
				if (this.strasse == null)
				{
					this.strasse = string.Empty;
				}
				
				return this.strasse;
			}
			
			set
			{
				this.strasse = value;
			}
		}
		
		/// <value>
		/// Gets or sets the Kreuzung object.
		/// </value>
		/// <summary>
		/// Gets or sets the Kreuzung object.
		/// </summary>
		public string Kreuzung
		{
			get
			{
				if (this.kreuzung == null)
				{
					this.kreuzung = string.Empty;
				}
				
				return this.kreuzung;
			}
			
			set
			{
				this.kreuzung = value;
			}
		}
		
		/// <value>
		/// Gets or sets the Ort object.
		/// </value>
		/// <summary>
		/// Gets or sets the Ort object.
		/// </summary>
		public string Ort
		{
			get
			{
				if (this.ort == null)
				{
					this.ort = string.Empty;
				}
				
				return this.ort;
			}
			
			set
			{
				this.ort = value;
			}
		}
		
		/// <value>
		/// Gets or sets the Objekt object.
		/// </value>
		/// <summary>
		/// Gets or sets the Objekt object.
		/// </summary>
		public string Objekt
		{
			get
			{
				if (this.objekt == null)
				{
					this.objekt = string.Empty;
				}
				
				return this.objekt;
			}
			
			set
			{
				this.objekt = value;
			}
		}
		
		/// <value>
		/// Gets or sets the Meldebild object.
		/// </value>
		/// <summary>
		/// Gets or sets the Meldebild object.
		/// </summary>
		public string Meldebild
		{
			get
			{
				if (this.meldebild == null)
				{
					this.meldebild = string.Empty;
				}
				
				return this.meldebild;
			}
			
			set
			{
				this.meldebild = value;
			}
		}
		
		/// <value>
		/// Gets or sets the Hinweis object.
		/// </value>
		/// <summary>
		/// Gets or sets the Hinweis object.
		/// </summary>
		public string Hinweis
		{
			get
			{
				if (this.hinweis == null)
				{
					this.hinweis = string.Empty;
				}
				
				return this.hinweis;
			}
			
			set
			{
				if (value.ToUpperInvariant().Contains("HAUPTMELDER"))
				{
					this.hinweis = value.Substring(value.IndexOf(" ", StringComparison.OrdinalIgnoreCase) + 1);
				}
				else
				{
					this.hinweis = value;
				}
			}
		}
		
		/// <value>
		/// Gets or sets the Einsatzplan object.
		/// </value>
		/// <summary>
		/// Gets or sets the Einsatzplan object.
		/// </summary>
		public string Einsatzplan
		{
			get
			{
				if (this.einsatzplan == null)
				{
					this.einsatzplan = string.Empty;
				}
				
				return this.einsatzplan;
			}
			
			set
			{
				this.einsatzplan = value;
			}
		}
		
		/// <value>
		/// Gets the Stichwort object.
		/// </value>
		/// <summary>
		/// Gets the Stichwort object.
		/// </summary>
		public string Stichwort
		{
			get
			{
				if (this.einsatznr != null)
				{
					if (this.einsatznr[0] == 'B')
					{
						return "Brand";
					}
					else if (this.einsatznr[0] == 'S')
					{
						return "Sontiges";
					}
					else if (this.einsatznr[0] == 'T')
					{
						return "THL";
					}
					else if (this.einsatznr[0] == 'R')
					{
						return "Rettungsdienst";
					}
				}
				
				return string.Empty;
			}
		}
	}
}
