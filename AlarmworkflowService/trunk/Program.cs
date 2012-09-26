// <copyright file="Program.cs" company="OpenFireSource.de">
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
// <summary>This File contains the Program.cs of the AlarmworkflowService.</summary>

namespace OpenFireSource.Alarmworkflow.AlarmworkflowService
{
	using System;
	using System.Collections.Generic;
	using System.ServiceProcess;
	using System.Text;
	
	/// <summary>
	/// Static class to start the AlarmworkflowService.
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// This method starts the service.
		/// </summary>
		public static void Main()
		{
			// To run more than one service you have to add them here
			ServiceBase.Run(new ServiceBase[] { new AlarmworkflowServiceClass() });
		}
	}
}
