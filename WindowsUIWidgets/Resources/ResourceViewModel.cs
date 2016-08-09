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
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AlarmWorkflow.BackendService.FileTransferContracts.Client;
using AlarmWorkflow.BackendService.ManagementContracts.Emk;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.UIContracts.ViewModels;

namespace AlarmWorkflow.Windows.UIWidgets.Resources
{
    /// <summary>
    /// Represents a ViewModel for displaying a single resource.
    /// </summary>
    public class ResourceViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets the underlying <see cref="OperationResource"/>-instance.
        /// </summary>
        public OperationResource Resource { get; private set; }
        /// <summary>
        /// Gets the icon to display. This may be empty if no icon was specified.
        /// </summary>
        public ImageSource Icon { get; private set; }

        public bool Dispatched => Resource == null && EmkResourceItem != null;

        /// <summary>
        /// Gets the display-friendly name of this resource, if configured.
        /// Otherwise returns the full name according to the <see cref="Resource"/>.
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (EmkResourceItem == null || string.IsNullOrWhiteSpace(EmkResourceItem.DisplayName))
                {
                    return Resource.FullName;
                }
                return EmkResourceItem.DisplayName;
            }
        }

        /// <summary>
        /// Gets the underlying <see cref="EmkResource"/>-instance
        /// </summary>
        public EmkResource EmkResourceItem { get; private set; }

        #endregion

        #region Constructors

        internal ResourceViewModel(OperationResource resource)
            : this(resource, null)
        {
        }

        internal ResourceViewModel(OperationResource resource, EmkResource emkResource)
            : base()
        {
            if (resource == null && emkResource == null)
            {
                throw new InvalidOperationException(Properties.Resources.NoResourceGiven);
            }
            this.Resource = resource;

            EmkResourceItem = emkResource;
            if (EmkResourceItem != null)
            {
                LoadIconAsync();
            }
        }

        #endregion

        #region

        private void LoadIconAsync()
        {
            ThreadPool.QueueUserWorkItem(w =>
            {
                using (FileTransferServiceClient client = new FileTransferServiceClient())
                {
                    try
                    {
                        Stream content = client.GetFileFromPath(EmkResourceItem.IconFileName);

                        if (content != null)
                        {
                            ImageSource icon = BitmapFrame.Create(content);
                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                Icon = icon;
                                OnPropertyChanged("Icon");
                            }));
                        }
                    }
                    catch (AssertionFailedException)
                    {
                        // Occurs if the path is empty.
                    }
                    catch (IOException)
                    {
                        // This exception is totally OK. No image will be displayed.
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.LogException(this, ex);
                    }
                }
            });
        }

        #endregion
    }
}
