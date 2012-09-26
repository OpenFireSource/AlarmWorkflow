// <copyright file="Window1.xaml.cs" company="OpenFireSource.de">
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
// <summary>This implements the Window1 class.</summary>

namespace AlarmworkflowApp
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Data;
	using System.Windows.Documents;
	using System.Windows.Input;
	using System.Windows.Media;
	using System.Windows.Media.Imaging;
	using System.Windows.Shapes;

	/// <summary>
	/// Interaction logic for Window1.xaml.
	/// </summary>
	public partial class Window1 : Window
	{
		/// <summary>
		/// Private instance of the AlarmworkflowCodeLib.
		/// </summary>
	    private OpenFireSource.Alarmworkflow.AlarmworkflowCodeLib.AlarmworkflowClass alarmworkflow;
	    
	    /// <summary>
	    /// Initializes a new instance of the Window1 class.
	    /// </summary>
		public Window1()
		{
			this.InitializeComponent();
			this.alarmworkflow = new OpenFireSource.Alarmworkflow.AlarmworkflowCodeLib.AlarmworkflowClass();
		}
		
		/// <summary>
		/// Delegate for Button pressed event of the start stop button.
		/// </summary>
		/// <param name="sender">Sender of this svent.</param>
		/// <param name="e">The RoutedEventArgs e.</param>
		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
		    if ((string)this.cmd_start.Content == "Start")
		    {
		        this.cmd_start.Content = "Stop";
		        this.alarmworkflow.Start();
		    }
        }
	}
}