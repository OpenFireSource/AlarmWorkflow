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
