using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmWorkflow.Parser.Library.util
{
    class GeographicCoords
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public static GeographicCoords FromGaussKrueger(double east, double north)
        {
            return GeoTransformation.GKPOD(east, north);
        }
    }
}
