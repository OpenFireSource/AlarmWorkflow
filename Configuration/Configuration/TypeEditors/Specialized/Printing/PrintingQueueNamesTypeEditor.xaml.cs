// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Specialized.Printing;
using AlarmWorkflow.Windows.ConfigurationContracts;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors.Specialized.Printing
{
    /// <summary>
    /// Interaction logic for PrintingQueueNamesTypeEditor.xaml
    /// </summary>
    [Export("PrintingQueueNamesTypeEditor", typeof(ITypeEditor))]
    public partial class PrintingQueueNamesTypeEditor : UserControl, ITypeEditor
    {
        #region Constants

        // Use "\n" because Environment.NewLine (\n\r) gets parsed to \n when deserializing XML. Alternative: Find fix.
        private static readonly string NewLineString = "\n";

        #endregion

        #region Properties

        /// <summary>
        /// Gets the array of available printing queue names.
        /// </summary>
        public IList<CheckedStringItem> PrintingQueues { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintingQueueNamesTypeEditor"/> class.
        /// </summary>
        public PrintingQueueNamesTypeEditor()
        {
            InitializeComponent();

            PrintingQueues = GetPrintingQueues().Entries
                .Select(pq => pq.Name)
                .OrderBy(p => p)
                .Select(n => new CheckedStringItem(n))
                .ToList();

            this.DataContext = this;
        }

        #endregion

        #region Methods

        private PrintingQueuesConfiguration GetPrintingQueues()
        {
            using (var service = ServiceFactory.GetCallbackServiceWrapper<ISettingsService>(new SettingsServiceCallback()))
            {
                return service.Instance.GetSetting(SettingKeys.PrintingQueuesConfiguration).GetValue<PrintingQueuesConfiguration>();
            }
        }

        #endregion

        #region ITypeEditor Members

        /// <summary>
        /// Gets/sets the value that is edited.
        /// </summary>
        public object Value
        {
            get
            {
                var selected = PrintingQueues.Where(n => n.IsChecked).Select(n => n.Value);
                return string.Join(NewLineString, selected);
            }
            set
            {
                string sv = (string)value;

                string[] selected = new string[0];
                if (!string.IsNullOrWhiteSpace(sv))
                {
                    selected = sv.Split(new string[] { NewLineString }, StringSplitOptions.None);
                }

                foreach (CheckedStringItem item in PrintingQueues)
                {
                    item.IsChecked = selected.Contains(item.Value);
                }
            }
        }

        /// <summary>
        /// Gets the visual element that is editing the value.
        /// </summary>
        public System.Windows.UIElement Visual
        {
            get { return this; }
        }

        void ITypeEditor.Initialize(string editorParameter)
        {
        }

        #endregion

    }
}