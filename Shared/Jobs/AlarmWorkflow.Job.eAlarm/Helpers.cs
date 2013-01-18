using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Xml.XPath;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Job.eAlarm
{
    static class Helpers
    {
        /// <summary>
        /// Returns the longitude and the latitude for a given address
        /// </summary>
        /// <param name="address">Address to search for</param>
        /// <returns>null or dictonary</returns>
        internal static Dictionary<string, string> GetGeocodes(string address)
        {
            Dictionary<string, string> geocodes = new Dictionary<string, string>();
            string urladdress = HttpUtility.UrlEncode(address);
            string url = "http://maps.googleapis.com/maps/api/geocode/xml?address=" + urladdress + "&sensor=false";

            WebResponse response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                response = request.GetResponse();
                XPathDocument document = new XPathDocument(response.GetResponseStream());
                XPathNavigator navigator = document.CreateNavigator();
                // get response status
                XPathNodeIterator statusIterator = navigator.Select("/GeocodeResponse/status");
                while (statusIterator.MoveNext())
                {
                    if (statusIterator.Current.Value != "OK")
                    {
                        return null;
                    }
                }
                // gets first restult
                XPathNodeIterator resultIterator = navigator.Select("/GeocodeResponse/result");
                resultIterator.MoveNext();
                XPathNodeIterator geometryIterator = resultIterator.Current.Select("geometry");
                geometryIterator.MoveNext();
                XPathNodeIterator locationIterator = geometryIterator.Current.Select("location");
                while (locationIterator.MoveNext())
                {
                    XPathNodeIterator latIterator = locationIterator.Current.Select("lat");
                    while (latIterator.MoveNext())
                    {
                        geocodes.Add(Properties.Resources.LATITUDE, latIterator.Current.Value);
                    }
                    XPathNodeIterator lngIterator = locationIterator.Current.Select("lng");
                    while (lngIterator.MoveNext())
                    {
                        geocodes.Add(Properties.Resources.LONGITUDE, lngIterator.Current.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, typeof(Helpers), "Could not retrieve geocode for address '{0}'.", address);
                Logger.Instance.LogException(typeof(Helpers), ex);
            }
            finally
            {

                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }

            return geocodes;
        }
    }
}

