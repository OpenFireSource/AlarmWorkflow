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
using System.Diagnostics;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.DefaultViewer.Properties;
using AlarmWorkflow.Windows.UIContracts;

namespace AlarmWorkflow.Windows.DefaultViewer.Converters
{
    /// <summary>
    /// 
    /// </summary>
    [ValueConversion(typeof(Operation), typeof(ImageSource))]
    public class OperationToRouteImageConverter : IValueConverter
    {
        #region Fields

        private static readonly string CacheLocation = Path.Combine(Utilities.GetLocalAppDataFolderPath(), "WinUIRouteCache");
        private static PropertyLocation FDLocation;

        #endregion

        #region Methods

        private static PropertyLocation GetFDLocation()
        {
            if (FDLocation == null)
            {
                FDLocation = new PropertyLocation();

                try
                {
                    using (var service = ServiceFactory.GetCallbackServiceWrapper<ISettingsService>(new SettingsServiceCallback()))
                    {
                        FDLocation.Street = service.Instance.GetSetting(SettingKeys.FDStreet).GetValue<string>();
                        FDLocation.StreetNumber = service.Instance.GetSetting(SettingKeys.FDStreetNumber).GetValue<string>();
                        FDLocation.ZipCode = service.Instance.GetSetting(SettingKeys.FDZipCode).GetValue<string>();
                        FDLocation.City = service.Instance.GetSetting(SettingKeys.FDCity).GetValue<string>();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogException(typeof(OperationToRouteImageConverter), ex);
                }
            }

            return FDLocation;
        }

        private byte[] DownloadRoutePlan(Operation operation)
        {
            PropertyLocation source = GetFDLocation();
            if (!source.IsMeaningful)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Resources.RoutePlanningSourceLocationNotMeaningful, source);
                return null;
            }

            PropertyLocation destination = operation.GetDestinationLocation();
            if (!operation.GetDestinationLocation().IsMeaningful)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Resources.DestinationLocationIsUnknown);
                return null;
            }

            Logger.Instance.LogFormat(LogType.Trace, this, Resources.DownloadRoutePlanBegin, destination.ToString());

            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                byte[] imageBuffer = GoogleMapsProvider.GetRouteImage(source, destination);

                sw.Stop();

                if (imageBuffer == null)
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, Resources.DownloadRoutePlanFailed);
                }
                else
                {
                    Logger.Instance.LogFormat(LogType.Trace, this, Resources.DownloadRoutePlanSuccess, sw.ElapsedMilliseconds);
                }

                return imageBuffer;
            }
            catch (Exception ex)
            {
                sw.Stop();
                Logger.Instance.LogFormat(LogType.Error, this, Resources.DownloadRoutePlanError);
                Logger.Instance.LogException(this, ex);
            }

            return null;
        }

        #endregion

        #region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Operation operation = value as Operation;
            if (operation == null)
            {
                return null;
            }

            byte[] buffer = null;
            FileInfo imagePath = new FileInfo(Path.Combine(CacheLocation, operation.Id + ".png"));
            if (!imagePath.Exists)
            {
                buffer = DownloadRoutePlan(operation);
                if (buffer == null)
                {
                    return ClientExtensions.GetPackUri(this, "Images/NoRouteImage.png");
                }

                imagePath.Directory.Create();
                File.WriteAllBytes(imagePath.FullName, buffer);
            }
            else
            {
                buffer = File.ReadAllBytes(imagePath.FullName);
            }

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new MemoryStream(buffer);
            image.EndInit();
            return image;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}