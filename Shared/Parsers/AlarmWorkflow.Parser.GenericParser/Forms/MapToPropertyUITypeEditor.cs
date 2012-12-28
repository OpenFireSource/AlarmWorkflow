using System;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using AlarmWorkflow.Shared.Core;
using System.Collections.Generic;

namespace AlarmWorkflow.Parser.GenericParser.Forms
{
    /// <summary>
    /// Logic of the UITypeEditor.
    /// </summary>
    internal partial class MapToPropertyUITypeEditor : UserControl
    {
        #region Constants

        private static readonly string[] DisallowedProperties = new[] { "CustomData", "IsAcknowledged", "Id", "Resources", "RouteImage" };
        private static readonly string[] OperationProperties;

        static MapToPropertyUITypeEditor()
        {
            List<string> propertiesTemp = new List<string>();
            FillAllowedProperties(typeof(Operation), "", propertiesTemp);
            propertiesTemp.Sort();

            OperationProperties = propertiesTemp.ToArray();
        }

        private static void FillAllowedProperties(Type child, string hierarchySoFar, IList<string> propertiesBucket)
        {
            foreach (PropertyInfo property in child.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (DisallowedProperties.Contains(property.Name))
                {
                    continue;
                }
                if (!property.CanWrite)
                {
                    continue;
                }

                // If the property may be extensible (just assume that blindly if it is not in the System-namespace)
                Type propertyType = property.PropertyType;
                if (!propertyType.Namespace.StartsWith("System"))
                {
                    FillAllowedProperties(propertyType, hierarchySoFar + property.Name + ".", propertiesBucket);
                }
                else
                {
                    string name = property.Name;
                    if (property.DeclaringType != null)
                    {
                        name = hierarchySoFar + name;
                    }

                    propertiesBucket.Add(name);
                }
            }
        }

        #endregion

        #region Fields

        private IWindowsFormsEditorService _service;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the property that was selected.
        /// </summary>
        public string PropertyName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(txtCustomDataProperty.Text))
                {
                    return txtCustomDataProperty.Text.Trim();
                }

                if (lsbSuggestions.SelectedItem != null)
                {
                    return (string)lsbSuggestions.SelectedItem;
                }
                return null;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MapToPropertyUITypeEditor"/> class.
        /// </summary>
        public MapToPropertyUITypeEditor()
        {
            InitializeComponent();

            lsbSuggestions.Items.AddRange(OperationProperties);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapToPropertyUITypeEditor"/> class.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="service">The service.</param>
        public MapToPropertyUITypeEditor(object value, IWindowsFormsEditorService service)
            : this()
        {
            _service = service;

            SelectValue(value);
        }

        #endregion

        #region Methods

        private void SelectValue(object value)
        {
            if (value == null)
            {
                return;
            }

            string v = (string)value;
            if (lsbSuggestions.Items.OfType<string>().Any(item => item == v))
            {
                lsbSuggestions.SelectedItem = v;
            }
            else
            {
                txtCustomDataProperty.Text = v;
            }
        }

        #endregion

        #region Event handlers

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            _service.CloseDropDown();
        }

        private void lsbSuggestions_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Select chosen item
            txtCustomDataProperty.Clear();

            _service.CloseDropDown();
        }

        #endregion

    }

    class MapToPropertyUITypeEditorImpl : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            if (context != null
               && context.Instance != null
               && provider != null)
            {

                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (edSvc != null)
                {
                    MapToPropertyUITypeEditor control = new MapToPropertyUITypeEditor(value, edSvc);
                    edSvc.DropDownControl(control);
                    return control.PropertyName;
                }
            }

            return value;
        }
    }
}
