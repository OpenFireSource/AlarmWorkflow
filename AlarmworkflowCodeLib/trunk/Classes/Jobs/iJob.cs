// <copyright file="iJob.cs" company="OpenFireSource.de">
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
// <summary>In this file the IJob interface is immplemented.</summary>
namespace OpenFireSource.Alarmworkflow.AlarmworkflowCodeLib.Jobs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	/// <summary>
	/// Thid interface descibes an job interface. Every job has to implement this Interface.
	/// </summary>
	public interface IJob
	{		
		/// <summary>
		/// Gets the error message.
		/// </summary>
		/// <value>
		/// Gets the error message.
		/// </value>
		string ErrorMessage
		{
			get;
		}
		
		/// <summary>
		/// This methode do the jobs job.
		/// </summary>
		/// <param name="einsatz">Current operation.</param>
		/// <returns>False when an error occured, otherwise true.</returns>
		bool DoJob(Einsatz einsatz);
	}
}
