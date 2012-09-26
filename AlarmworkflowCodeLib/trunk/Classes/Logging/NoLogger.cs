// <copyright file="NoLogger.cs" company="OpenFireSource.de">
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
// <summary>In this file the NoLogger class is immplemented.</summary>

namespace OpenFireSource.Alarmworkflow.AlarmworkflowCodeLib.Logging
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	/// <summary>
	/// The NoLoger class, log all events to nothing.
	/// </summary>
	public class NoLogger : ILogger
	{
		/// <summary>
		/// Inherited by iLogger abstract class. Initializes the logger.
		/// </summary>
		/// <returns>Indicates if an error occured. Always false.</returns>
		public override bool InitLogging()
		{
			return false;
		}

		/// <summary>
		/// Writes some information to nothing.
		/// </summary>
		/// <param name="info">The information which not will be loged.</param>
		public override void WriteInformation(string info)
		{
		}

		/// <summary>
		/// Writes some warning to nothing.
		/// </summary>
		/// <param name="warning">The warning which not will be loged.</param>
		public override void WriteWarning(string warning)
		{
		}

		/// <summary>
		/// Writes some error to nothing.
		/// </summary>
		/// <param name="errorMessage">The error which not will be loged.</param>
		public override void WriteError(string errorMessage)
		{
		}
	}
}
