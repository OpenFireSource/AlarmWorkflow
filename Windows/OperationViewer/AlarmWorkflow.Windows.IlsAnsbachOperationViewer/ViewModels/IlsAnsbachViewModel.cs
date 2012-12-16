using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Windows.IlsAnsbachOperationViewer.ViewModels;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.IlsAnsbachOperationViewer
{
    class IlsAnsbachViewModel : ViewModelBase
    {
        #region Fields

        private Operation _operation;

        private UIConfiguration _configuration;

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
        /// Gets a list containing the resources requested for this operation.
        /// </summary>
        public IEnumerable<ResourceViewModel> VehicleResources
        {
            get { return GetResources(); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IlsAnsbachViewModel"/> class.
        /// </summary>
        public IlsAnsbachViewModel()
        {
            _configuration = UIConfiguration.Load();
        }

        #endregion

        #region Methods

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
            foreach (var pi in this.GetType().GetProperties().Where(p => p.Name != "Operation"))
            {
                OnPropertyChanged(pi.Name);
            }
        }

        private IEnumerable<ResourceViewModel> GetResources()
        {
            List<ResourceViewModel> resources = new List<ResourceViewModel>();

            if (_operation != null && _operation.Resources != null)
            {
                foreach (OperationResource resource in _operation.Resources)
                {
                    // Check if the filter matches
                    var vehicle = _configuration.FindMatchingResource(resource.FullName);
                    if (vehicle == null)
                    {
                        continue;
                    }

                    ResourceViewModel rvm = resources.FirstOrDefault(t => t.VehicleName == vehicle.Name);
                    if (rvm == null)
                    {
                        // Sometimes the vehicles get requested multiple times and it differs only in the equipment needed
                        // So we just create the vehicle once and add the equipment to that
                        rvm = new ResourceViewModel();
                        rvm.VehicleName = vehicle.Name;
                        rvm.SetImage(vehicle.Image);
                        resources.Add(rvm);
                    }

                    if (rvm.RequestedEquipment != null)
                    {
                        // Add newline when adding equipment to already existing vehicle
                        rvm.RequestedEquipment += "\n";
                    }
                    rvm.RequestedEquipment += string.Join("\n", resource.RequestedEquipment);

                }
            }

            return resources.OrderBy(r => r.VehicleName);
        }

        #endregion
    }
}
