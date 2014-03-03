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

using System.Windows.Controls;
using AlarmWorkflow.BackendService.AddressingContracts.EntryObjects;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.Configuration.AddressBookEditor.Extensibility;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.AddressBookEditor.CustomDataEditors
{
    /// <summary>
    /// Interaction logic for MobilePhoneCustomDataEditor.xaml
    /// </summary>
    [Export("MobilePhoneCustomDataEditor", typeof(ICustomDataEditor))]
    [Information(DisplayName = "INF_MobilePhoneCustomDataEditor", Tag = "MobilePhone")]
    public partial class MobilePhoneCustomDataEditor : UserControl, ICustomDataEditor
    {
        #region Properties

        /// <summary>
        /// Gets/sets the phone number.
        /// </summary>
        public string PhoneNumber { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MobilePhoneCustomDataEditor"/> class.
        /// </summary>
        public MobilePhoneCustomDataEditor()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        #endregion

        #region ITypeEditor Members

        object ITypeEditor.Value
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PhoneNumber))
                {
                    return null;
                }

                MobilePhoneEntryObject meo = new MobilePhoneEntryObject();
                meo.PhoneNumber = PhoneNumber;
                return meo;
            }
            set
            {
                MobilePhoneEntryObject meo = (MobilePhoneEntryObject)value;
                this.PhoneNumber = meo.PhoneNumber;
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