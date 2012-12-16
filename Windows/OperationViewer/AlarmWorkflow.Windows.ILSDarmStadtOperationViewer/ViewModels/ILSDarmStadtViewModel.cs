using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.ILSDarmStadtOperationViewer
{
    class ILSDarmStadtOperationViewModel : ViewModelBase
    {
        #region Fields

        private Operation _operation;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        public Operation Operation
        {
            get { return _operation; }
            set
            {
                _operation = value;

                // Set operation itself
                OnPropertyChanged("Operation");

                // Update all other properties
                UpdateProperties();
            }
        }

        /// <summary>
        /// Gets the route plan image.
        /// </summary>
        public ImageSource RoutePlanImage
        {
            get { return GetRoutePlanImage(); }
        }

        /// <summary>
        /// Gets the value from the "FurtherInformation" custom data field.
        /// </summary>
        public string FurtherInformation
        {
            get { return GetOperationCustomData<string>("Zusatzinfos", null); }
        }

        /// <summary>
        /// Gets the value from the "Location" custom data field.
        /// </summary>
        public string Location
        {
            get { return _operation.Einsatzort.Street + " " + _operation.Einsatzort.StreetNumber + " " + _operation.Einsatzort.City; }
        }

        /// <summary>
        /// Gets the value from the "Betroffene" custom data field.
        /// </summary>
        public string Betroffene
        {
            get { return GetOperationCustomData<string>("Betroffene", null); }
        }

        /// <summary>
        /// Gets the value from the "Sondersignal" custom data field.
        /// </summary>
        public string Sondersignal
        {
            get { return GetOperationCustomData<string>("Sondersignal", null); }
        }
        /// <summary>
        /// Gets the value from the "Loops" custom data field.
        /// </summary>
        public string Loops
        {
            get { return _operation.Resources.ToString("{FullName} | {RequestedEquipment}", null); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ILSDarmStadtOperationViewModel"/> class.
        /// </summary>
        public ILSDarmStadtOperationViewModel()
        {
            UIConfiguration.Load();
        }

        #endregion

        #region Methods

        private T GetOperationCustomData<T>(string key, T defaultValue)
        {
            if (_operation != null && _operation.CustomData != null && _operation.CustomData.ContainsKey(key))
            {
                return (T)_operation.CustomData[key];
            }

            return defaultValue;
        }

        private ImageSource GetRoutePlanImage()
        {
            if (_operation == null)
            {
                return null;
            }
            if (_operation.RouteImage == null)
            {
                // Return dummy image
                return Helper.GetNoRouteImage();
            }

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new MemoryStream(_operation.RouteImage);
            image.EndInit();
            return image;
        }

        private void UpdateProperties()
        {
            foreach (var pi in GetType().GetProperties().Where(p => p.Name != "Operation"))
            {
                OnPropertyChanged(pi.Name);
            }
        }

        #endregion
    }
}
