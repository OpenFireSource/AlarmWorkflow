// This file is part of the GeoUtility by Steffen Habermehl.
// 
// GeoUtility is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// You should have received a copy of the GNU General Public License
// along with GeoUtility.  If not, see <http://www.gnu.org/licenses/>.

using System;

namespace AlarmWorkflow.Parser.Library.util.geo.Transform
{
    internal partial class Transformation
    {

        /// <summary><para>Die Funktion verschiebt das Datum von <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84</see> (international) nach <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.Potsdam">Potsdam</see> (nur Deutschland).
        /// <para>Die Funktion ist nur für interne Berechnungen bestimmt.</para></para></summary>
        /// <remarks><para>
        /// Hintergründe zum Problem der Koordinatentransformationen sowie entsprechende  mathematische 
        /// Formeln können den einschlägigen Fachbüchern oder dem Internet entnommen werden.<p />
        /// Quellen: 
        /// Bundesamt für Kartographie und Geodäsie<br />
        /// <a href="http://www.bkg.bund.de" target="_blank">http://www.bkg.bund.de</a><br />
        /// <a href="http://crs.bkg.bund.de" target="_blank">http://crs.bkg.bund.de</a><br />
        /// </para></remarks>
        ///         
        /// <param name="geo">Ein <see cref="GeoUtility.GeoSystem.Geographic"/>-Objekt im <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84">WGS84-Datum</see>.</param>
        /// <returns>Ein <see cref="GeoUtility.GeoSystem.Geographic"/>-Objekt im <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.Potsdam">Potsdam-Datum</see>.</returns>
        public static Geographic WGSPOD(Geographic geo)
        {

            double laengeWGS84 = geo.Longitude;
            double breiteWGS84 = geo.Latitude;

            // Breite und Länge (Rad)
            double breiteRad = breiteWGS84 * (Math.PI / 180);
            double laengeRad = laengeWGS84 * (Math.PI / 180);

            // Querkrümmung
            double qkhm = WGS84_HALBACHSE / Math.Sqrt(1 - WGS84_EXZENT * Math.Sin(breiteRad) * Math.Sin(breiteRad));

            // Kartesische Koordinaten WGS84
            double xWGS84 = qkhm * Math.Cos(breiteRad) * Math.Cos(laengeRad);
            double yWGS84 = qkhm * Math.Cos(breiteRad) * Math.Sin(laengeRad);
            double zWGS84 = (1 - WGS84_EXZENT) * qkhm * Math.Sin(breiteRad);

            // Kartesische Koordinaten Potsdam
            double x = xWGS84 - POTSDAM_DATUM_SHIFT_X;
            double y = yWGS84 - POTSDAM_DATUM_SHIFT_Y;
            double z = zWGS84 - POTSDAM_DATUM_SHIFT_Z;

            // Breite und Länge im Potsdam Datum
            double b = Math.Sqrt(x * x + y * y);
            double breite = (180 / Math.PI) * Math.Atan((z / b) / (1 - EXZENT));

            double laenge = 0;
            if (x > 0) laenge = (180 / Math.PI) * Math.Atan(y / x);
            if (x < 0 && y > 0) laenge = (180 / Math.PI) * Math.Atan(y / x) + 180;
            if (x < 0 && y < 0) laenge = (180 / Math.PI) * Math.Atan(y / x) - 180;

            if (laenge < MIN_OST || laenge > MAX_OST || breite < MIN_NORD || breite > MAX_NORD)
            {
                throw new ArgumentException("ERROR_GK_OUT_OF_RANGE");
            }

            return new Geographic(laenge, breite, GeoDatum.Potsdam);
        }

    }
}
