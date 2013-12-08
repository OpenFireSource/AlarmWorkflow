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
using System.Windows;
using System.Windows.Controls;
using AlarmWorkflow.BackendService.AddressingContracts.EntryObjects;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.Configuration.AddressBookEditor.Extensibility;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.AddressBookEditor.CustomDataEditors
{
    /// <summary>
    /// Interaction logic for MailCustomDataEditor.xaml
    /// </summary>
    [Export("MailCustomDataEditor", typeof(ICustomDataEditor))]
    [Information(DisplayName = "INF_MailCustomDataEditor", Tag = "Mail")]
    public partial class MailCustomDataEditor : UserControl, ICustomDataEditor
    {
        #region Properties

        /// <summary>
        /// Gets/sets the e-mail address.
        /// </summary>
        public string MailAddress { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MailCustomDataEditor"/> class.
        /// </summary>
        public MailCustomDataEditor()
        {
            InitializeComponent();

            this.DataContext = this;

            foreach (string name in Enum.GetNames(typeof(MailAddressEntryObject.ReceiptType)))
            {
                cboReceiptType.Items.Add(name);
            }
            cboReceiptType.SelectedIndex = 0;
        }

        #endregion

        #region ITypeEditor Members

        object ITypeEditor.Value
        {
            get
            {
                string address = MailAddress;
                string receiptType = (string)cboReceiptType.SelectedItem;

                // TODO: Verify format first! The method below will return "null" if parsing failed.
                MailAddressEntryObject meo = MailAddressEntryObject.FromAddress(address, receiptType);

                return meo;
            }
            set
            {
                MailAddressEntryObject meo = (MailAddressEntryObject)value;

                MailAddress = meo.Address.Address;
                cboReceiptType.Text = meo.Type.ToString();
            }
        }

        /// <summary>
        /// Gets the visual element that is editing the value.
        /// </summary>
        public UIElement Visual
        {
            get { return this; }
        }

        void ITypeEditor.Initialize(string editorParameter)
        {

        }

        #endregion
    }
}