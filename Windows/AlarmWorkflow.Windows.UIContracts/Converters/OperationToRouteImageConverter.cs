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
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Windows.UIContracts.Properties;

namespace AlarmWorkflow.Windows.UIContracts.Converters
{
    /// <summary>
    /// 
    /// </summary>
    [ValueConversion(typeof(Operation), typeof(ImageSource))]
    public class OperationToRouteImageConverter : IValueConverter
    {
        #region Fields

        private static readonly string CacheLocation = Path.Combine(Utilities.GetLocalAppDataFolderPath(), "WinUIRouteCache");

        #endregion

        #region Methods

        private byte[] DownloadRoutePlan(Operation operation)
        {
            PropertyLocation source = AlarmWorkflowConfiguration.Instance.FDInformation.Location;
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
                    return Helper.GetNoRouteImage();
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