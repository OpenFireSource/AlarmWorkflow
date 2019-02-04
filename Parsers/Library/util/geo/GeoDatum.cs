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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmWorkflow.Parser.Library.util.geo
{

    /// <summary><para>Enumeration Geodatum: <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.WGS84"/> (International), 
    /// <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.Potsdam">Potsdam-Datum</see> (nur Deutschland)</para></summary>
    /// <remarks><para>
    /// Wird eine Koordinatentransformation in <see cref="GeoUtility.GeoSystem.GaussKrueger">Gauss-Krüger</see> 
    /// Koordinatensystem durchgeführt, so müssen die Koordinaten des <see cref="GeoUtility.GeoSystem.Geographic">Längen-/Breitensystems</see> 
    /// im <see cref="Potsdam">Potsdam-Datum</see> vorliegen. Alle anderen Systeme benutzen das Datum WGS84. 
    /// Bei der Transformation eines Koordinatensystems in das <see cref="GeoUtility.GeoSystem.GaussKrueger">Gauss-Krüger</see> 
    /// System müssen die Koordinaten also in das <see cref="GeoUtility.GeoSystem.Helper.GeoDatum.Potsdam">Potsdam-Datum</see> 
    /// gebracht werden. Dies geschieht jedoch normalerweise bei der Typkonvertierung automatisch. 
    /// <para>Hintergründe dazu siehe auch den Wikipedia-Artikel <i>Geodätisches Datum</i> unter dem Link: <br />
    /// <a href="http://de.wikipedia.org/wiki/Geod%C3%A4tisches_Datum" target="_blank">http://de.wikipedia.org/wiki/Geod%C3%A4tisches_Datum</a></para>
    /// </para></remarks>
    /// <seealso cref="GeoUtility.GeoSystem.Geographic.SetDatum"/>
    /// 
    /// <example>Dieses Beispiel zeigt das explizite Setzen des Datums bei einer Konvertierung von einem
    /// <see cref="GeoUtility.GeoSystem.Geographic">Längen-/Breitensystem</see> in ein 
    /// <see cref="GeoUtility.GeoSystem.GaussKrueger">Gauss-Krüger-System</see>. <para>Bitte beachten: Das Setzen 
    /// des Datums geschieht bei der Typkonvertierung automatisch, ist also nicht erforderlich. Bei der Rückkonvertierung 
    /// von <see cref="GeoUtility.GeoSystem.GaussKrueger">Gauss-Krüger</see> in das Längen-Breiten-System, wird 
    /// automatisch das geodätische Datum <see cref="WGS84"/> gesetzt.</para>
    /// <code>
    /// GeoUtility.GeoSystem.Geographic geo = new GeoUtility.GeoSystem.Geographic(8.12345, 50.54321);
    /// geo.SetDatum(GeoDatum.Potsdam)
    /// GeoUtility.GeoSystem.GaussKrueger gk = (GeoUtility.GeoSystem.GaussKrueger)geo;
    /// </code>
    /// </example>
    public enum GeoDatum
    {
        /// <summary><para>Geodätisches Datum <see cref="WGS84"/> (International)</para></summary>
        WGS84,

        /// <summary><para>Geodätisches Datum <see cref="Potsdam"/> (nur für Deutschland gültig)</para></summary>
        Potsdam
    }
}
