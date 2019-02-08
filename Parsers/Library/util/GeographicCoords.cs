using AlarmWorkflow.Parser.Library.util.geo;

namespace AlarmWorkflow.Parser.Library.util
{
    class GeographicCoords
    {
        public static Geographic FromGaussKrueger(double east, double north)
        {
            GaussKrueger gauss = new GaussKrueger(east, north);
            return (Geographic)gauss;
        }
    }
}
