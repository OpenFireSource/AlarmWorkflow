// <copyright file="iLogger.cs" company="OpenFireSource.de">
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
// <summary>In this file the iLogger class is immplemented.</summary>

namespace OpenFireSource.Alarmworkflow.AlarmworkflowCodeLib.Logging
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	/// <summary>
	/// This class must be inherit by all Loggers.
	/// </summary>
	public abstract class ILogger
	{
		/// <summary>
		/// Indicates if the Logger is enabled.
		/// </summary>
		private bool enabled;

		/// <summary>
		/// Gets or sets a value indicating whether the Logger is enabled or not.
		/// </summary>
		/// <value>
		/// Gets or sets a value indicating whether the Logger is enabled or not.
		/// </value>
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			
			set
			{
				this. enabled = value;
			}
		}

		/// <summary>
		/// Initializes the logger. Must be called first.
		/// </summary>
		/// <returns>Indicates if an error occured.</returns>
		public abstract bool InitLogging();
		
		/// <summary>
		/// Writes some information.
		/// </summary>
		/// <param name="info">The information which will be loged.</param>
		public abstract void WriteInformation(string info);
		
		/// <summary>
		/// Writes some warning.
		/// </summary>
		/// <param name="warning">The warning which will be loged.</param>
		public abstract void WriteWarning(string warning);
		
		/// <summary>
		/// Writes some error.
		/// </summary>
		/// <param name="errorMessage">The error which will be loged.</param>
		public abstract void WriteError(string errorMessage);
	}
}
