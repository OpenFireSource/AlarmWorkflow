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

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.ConfigurationContracts;

namespace AlarmWorkflow.Windows.Configuration.TypeEditors
{
    /// <summary>
    /// Interaktionslogik für ColorTypeEditor.xaml
    /// </summary>
    [Export("ColorTypeEditor", typeof(ITypeEditor))]
    public partial class ColorTypeEditor : ITypeEditor, INotifyPropertyChanged
    {
        #region Fields

        private ColorItem _selectedColor;

        #endregion

        #region Properties

        /// <summary>
        /// A list of <see cref="ColorItem"/>s for the Combobox.
        /// </summary>
        public ObservableCollection<ColorItem> ColorList { get; set; }

        /// <summary>
        /// Gets/sets the selected color.
        /// </summary>
        public ColorItem SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                _selectedColor = value;
                OnPropertyChanged("SelectedColor");
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorTypeEditor"/> class.
        /// </summary>
        public ColorTypeEditor()
        {
            InitializeComponent();
            DataContext = this;
            ColorList = new ObservableCollection<ColorItem>(typeof(Colors)
                                                      .GetProperties(BindingFlags.Static | BindingFlags.Public)
                                                      .Select(p => new ColorItem()
                                                          {
                                                              Name = p.Name,
                                                              Brush = new SolidColorBrush((Color)p.GetValue(null, null))
                                                          }
                                                      ).ToList());
            OnPropertyChanged("ColorList");

        }

        #endregion

        #region ITypeEditor Members

        void ITypeEditor.Initialize(string editorParameter)
        {
        }

        /// <summary>
        /// Gets/sets the value that is edited.
        /// </summary>
        public object Value
        {
            get
            {
                return SelectedColor.Brush.Color.ToString();
            }
            set
            {
                object convertFromString = ColorConverter.ConvertFromString((string)value);
                if (convertFromString != null)
                {
                    Color color = (Color)convertFromString;
                    ColorItem item = ColorList.FirstOrDefault(coloritem => coloritem.Brush.Color == color);
                    if (item != null)
                    {
                        SelectedColor = item;
                        OnPropertyChanged("SelectedColor");
                    }
                }
            }
        }

        /// <summary>
        /// Gets the visual element that is editing the value.
        /// </summary>
        public UIElement Visual
        {
            get { return this; }
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when the value of a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Nested types

        /// <summary>
        /// ColorItem for the <see cref="ColorTypeEditor"/>.
        /// </summary>
        public class ColorItem
        {
            /// <summary>
            /// Gets/sets the brush of the ColorItem
            /// </summary>
            public SolidColorBrush Brush { get; set; }

            /// <summary>
            /// Gets/sets the name of the ColorItem
            /// </summary>
            public string Name { get; set; }
        }

        #endregion
    }

}
