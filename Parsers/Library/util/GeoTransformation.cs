using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmWorkflow.Parser.Library.util
{
    /// <summary>
    /// This method is a copy from https://archive.codeplex.com/?p=geoutility
    /// </summary>
    internal class GeoTransformation
    {
        #region ===================== Gauss-Krueger =====================

        // Große Halbachse und Abplattung BESSEL
        const double HALBACHSE = 6377397.155;
        const double ABPLATTUNG = 3.342773182E-03;

        // Polkrümmung
        const double POL = HALBACHSE / (1 - ABPLATTUNG);

        // Num. Exzentrizitäten
        const double EXZENT = ((2 * ABPLATTUNG) - (ABPLATTUNG * ABPLATTUNG));
        const double EXZENT2 = ((2 * ABPLATTUNG) - (ABPLATTUNG * ABPLATTUNG)) / ((1 - ABPLATTUNG) * (1 - ABPLATTUNG));
        const double EXZENT4 = EXZENT2 * EXZENT2;
        const double EXZENT6 = EXZENT4 * EXZENT2;
        const double EXZENT8 = EXZENT4 * EXZENT4;

        // Geographische Grenzen des Gauss-Krüger-Systems in Grad
        const double MIN_OST = 5.0;
        const double MAX_OST = 16.0;
        const double MIN_NORD = 46.0;
        const double MAX_NORD = 56.0;
       
        #endregion ===================== Gauss-Krueger =====================

        /// <summary><para>Die Funktion wandelt die Koordinaten eines <see cref="GeoUtility.GeoSystem.GaussKrueger"/>-Objektes 
        /// in Längen/Breiten-Koordinaten eines <see cref="GeoUtility.GeoSystem.Geographic"/>-Objekts im Potsdam-Datum um. 
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
        /// <param name="gauss">Ein <see cref="GeoUtility.GeoSystem.GaussKrueger"/>-Objekt-</param>
        /// <returns>Ein <see cref="GeoUtility.GeoSystem.Geographic"/>-Objekt (<see cref="GeoUtility.GeoSystem.Helper.GeoDatum.Potsdam">Potsdam-Datum</see>).</returns>
        internal static GeographicCoords GKPOD(double rechts, double hoch)
        {
            // Koeffizienten für Länge Meridianbogen
            double koeff0 = POL * (Math.PI / 180) * (1 - 3 * EXZENT2 / 4 + 45 * EXZENT4 / 64 - 175 * EXZENT6 / 256 + 11025 * EXZENT8 / 16384);
            double koeff2 = (180 / Math.PI) * (3 * EXZENT2 / 8 - 3 * EXZENT4 / 16 + 213 * EXZENT6 / 2048 - 255 * EXZENT8 / 4096);
            double koeff4 = (180 / Math.PI) * (21 * EXZENT4 / 256 - 21 * EXZENT6 / 256 + 533 * EXZENT8 / 8192);
            double koeff6 = (180 / Math.PI) * (151 * EXZENT6 / 6144 - 453 * EXZENT8 / 12288);

            // Geogr. Breite (Rad)
            double sig = hoch / koeff0;
            double sigRad = sig * Math.PI / 180;
            double fbreite = sig + koeff2 * Math.Sin(2 * sigRad) + koeff4 * Math.Sin(4 * sigRad) + koeff6 * Math.Sin(6 * sigRad);
            double breiteRad = fbreite * Math.PI / 180;

            double tangens1 = Math.Tan(breiteRad);
            double tangens2 = Math.Pow(tangens1, 2);
            double tangens4 = Math.Pow(tangens1, 4);
            double cosinus1 = Math.Cos(breiteRad);
            double cosinus2 = Math.Pow(cosinus1, 2);

            double eta = EXZENT2 * cosinus2;

            // Querkrümmung
            double qkhm1 = POL / Math.Sqrt(1 + eta);
            double qkhm2 = Math.Pow(qkhm1, 2);
            double qkhm3 = Math.Pow(qkhm1, 3);
            double qkhm4 = Math.Pow(qkhm1, 4);
            double qkhm5 = Math.Pow(qkhm1, 5);
            double qkhm6 = Math.Pow(qkhm1, 6);

            // Differenz zum Bezugsmeridian
            int kfakt = (int)(rechts / 1E+06);
            int merid = kfakt * 3;
            double dlaenge1 = rechts - (kfakt * 1E+06 + 500000);
            double dlaenge2 = Math.Pow(dlaenge1, 2);
            double dlaenge3 = Math.Pow(dlaenge1, 3);
            double dlaenge4 = Math.Pow(dlaenge1, 4);
            double dlaenge5 = Math.Pow(dlaenge1, 5);
            double dlaenge6 = Math.Pow(dlaenge1, 6);

            // Faktor für Berechnung Breite
            double bfakt2 = -tangens1 * (1 + eta) / (2 * qkhm2);
            double bfakt4 = tangens1 * (5 + 3 * tangens2 + 6 * eta * (1 - tangens2)) / (24 * qkhm4);
            double bfakt6 = -tangens1 * (61 + 90 * tangens2 + 45 * tangens4) / (720 * qkhm6);

            // Faktor für Berechnung Länge
            double lfakt1 = 1 / (qkhm1 * cosinus1);
            double lfakt3 = -(1 + 2 * tangens2 + eta) / (6 * qkhm3 * cosinus1);
            double lfakt5 = (5 + 28 * tangens2 + 24 * tangens4) / (120 * qkhm5 * cosinus1);

            // Geographische Länge und Breite Potsdam
            double geoBreite = fbreite + (180 / Math.PI) * (bfakt2 * dlaenge2 + bfakt4 * dlaenge4 + bfakt6 * dlaenge6);
            double geoLaenge = merid + (180 / Math.PI) * (lfakt1 * dlaenge1 + lfakt3 * dlaenge3 + lfakt5 * dlaenge5);

            if (geoLaenge < MIN_OST || geoLaenge > MAX_OST || geoBreite < MIN_NORD || geoBreite > MAX_NORD)
            {
                throw new ArgumentException("ERROR_GK_OUT_OF_RANGE");
            }

            return new GeographicCoords()
            {
                Longitude = geoLaenge,
                Latitude =  geoBreite
            };
        }
    }
}
