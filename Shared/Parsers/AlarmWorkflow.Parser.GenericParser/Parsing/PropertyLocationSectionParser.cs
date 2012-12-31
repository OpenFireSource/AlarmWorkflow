using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Parser.GenericParser.Parsing
{
    /// <summary>
    /// Provides a section parser that parses a section describing a location.
    /// </summary>
    [Export("PropertyLocationSectionParser", typeof(ISectionParser))]
    public class PropertyLocationSectionParser : ISectionParser
    {
        #region Properties

        /// <summary>
        /// Gets/sets the location-object that is affected by this parser.
        /// </summary>
        [Option("Zugewiesener Ort")]
        public PropertyLocationTarget AffectedLocation { get; set; }
        /// <summary>
        /// Gets/sets the keyword that designates the "Street" parameter.
        /// </summary>
        [Option("Straße", Category = "Schlüsselwort")]
        public string KeywordStreet { get; set; }
        /// <summary>
        /// Gets/sets the keyword that designates the "StreetNumber" parameter.
        /// </summary>
        [Option("Hausnummer", Category = "Schlüsselwort")]
        public string KeywordStreetNumber { get; set; }
        /// <summary>
        /// Gets/sets the keyword that designates the "City" parameter.
        /// </summary>
        [Option("Ort", Category = "Schlüsselwort")]
        public string KeywordCity { get; set; }
        /// <summary>
        /// Gets/sets the keyword that designates the "Intersection" parameter.
        /// </summary>
        [Option("Kreuzung", Category = "Schlüsselwort")]
        public string KeywordIntersection { get; set; }
        /// <summary>
        /// Gets/sets the keyword that designates the "Property" parameter.
        /// </summary>
        [Option("Objekt", Category = "Schlüsselwort")]
        public string KeywordProperty { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyLocationSectionParser"/> class.
        /// </summary>
        public PropertyLocationSectionParser()
        {
            KeywordStreet = "Straße";
            KeywordStreetNumber = "Haus-Nr";
            KeywordCity = "Ort";
            KeywordIntersection = "Kreuzung";
            KeywordProperty = "Objekt";
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attempts to read the zip code from the city, if available.
        /// </summary>
        /// <param name="cityText"></param>
        /// <returns>The zip code of the city. -or- null, if there was no.</returns>
        private string ReadZipCodeFromCity(string cityText)
        {
            string zipCode = "";
            foreach (char c in cityText)
            {
                if (char.IsNumber(c))
                {
                    zipCode += c;
                    continue;
                }
                break;
            }
            return zipCode;
        }

        #endregion

        #region ISectionParser Members

        void ISectionParser.OnEnterSection(Operation operation)
        {
        }

        void ISectionParser.OnLeaveSection(Operation operation)
        {
        }

        void ISectionParser.Populate(AreaToken token, Operation operation)
        {
            // Determine which location to populate
            PropertyLocation location = null;
            switch (AffectedLocation)
            {
                case PropertyLocationTarget.Einsatzort:
                    location = operation.Einsatzort;
                    break;
                case PropertyLocationTarget.Zielort:
                    location = operation.Zielort;
                    break;
                default:
                    break;
            }

            string msg = token.Value;
            if (token.Identifier == KeywordStreet)
            {
                // The street here is mangled together with the street number. Dissect them...
                int streetNumberColonIndex = msg.LastIndexOf(':');
                if (streetNumberColonIndex != -1)
                {
                    // We need to check for occurrence of the colon, because it may have been omitted by the OCR-software
                    string streetNumber = msg.Remove(0, streetNumberColonIndex + 1).Trim();
                    location.StreetNumber = streetNumber;
                }

                location.Street = msg.Substring(0, msg.IndexOf("Haus-")).Trim();

            }
            else if (token.Identifier == KeywordStreetNumber)
            {
                location.StreetNumber = msg;
            }
            else if (token.Identifier == KeywordCity)
            {
                location.ZipCode = ReadZipCodeFromCity(msg);
                if (string.IsNullOrWhiteSpace(location.ZipCode))
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, "Could not find a zip code for city '{0}'. Route planning may fail or yield wrong results!", location.City);
                }

                location.City = msg.Remove(0, location.ZipCode.Length).Trim();

                // The City-text often contains a dash after which the administrative city appears multiple times (like "City A - City A City A").
                // However we can (at least with google maps) omit this information without problems!
                int dashIndex = location.City.IndexOf('-');
                if (dashIndex != -1)
                {
                    // Ignore everything after the dash
                    location.City = location.City.Substring(0, dashIndex);
                }
            }
            else if (token.Identifier == KeywordIntersection)
            {
                location.Intersection = msg;
            }
            else if (token.Identifier == KeywordProperty)
            {
                location.Property = msg;
            }
            //case "PLANNUMMER":
            //    operation.CustomData["Einsatzort Plannummer"] = msg;
            //    break;
            //case "STATION":
            //    operation.CustomData["Einsatzort Station"] = msg;
            //    break;
        }

        #endregion

        #region Nested types

        public enum PropertyLocationTarget
        {
            Einsatzort,
            Zielort,
        }

        #endregion
    }
}
