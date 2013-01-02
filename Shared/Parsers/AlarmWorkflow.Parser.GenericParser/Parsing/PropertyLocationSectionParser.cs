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
        public string AffectedLocation { get; set; }
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
            AffectedLocation = "Einsatzort";
            KeywordStreet = "Straße";
            KeywordStreetNumber = "Haus-Nr.";
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

        System.Collections.Generic.IEnumerable<string> ISectionParser.GetTokens()
        {
            yield return KeywordStreet;
            yield return KeywordStreetNumber;
            yield return KeywordCity;
            yield return KeywordIntersection;
            yield return KeywordProperty;
        }

        void ISectionParser.OnLoad(System.Collections.Generic.IDictionary<string, string> parameters)
        {
            AffectedLocation = parameters.SafeGetValue("AffectedLocation", AffectedLocation);
            KeywordStreet = parameters.SafeGetValue("KeywordStreet", KeywordStreet);
            KeywordStreetNumber = parameters.SafeGetValue("KeywordStreetNumber", KeywordStreetNumber);
            KeywordCity = parameters.SafeGetValue("KeywordCity", KeywordCity);
            KeywordIntersection = parameters.SafeGetValue("KeywordIntersection", KeywordIntersection);
            KeywordProperty = parameters.SafeGetValue("KeywordProperty", KeywordProperty);
        }

        void ISectionParser.OnSave(System.Collections.Generic.IDictionary<string, string> parameters)
        {
            parameters.Add("AffectedLocation", AffectedLocation);
            parameters.Add("KeywordStreet", KeywordStreet);
            parameters.Add("KeywordStreetNumber", KeywordStreetNumber);
            parameters.Add("KeywordCity", KeywordCity);
            parameters.Add("KeywordIntersection", KeywordIntersection);
            parameters.Add("KeywordProperty", KeywordProperty);
        }

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
                case "Zielort":
                    location = operation.Zielort;
                    break;
                default:
                case "Einsatzort":
                    location = operation.Einsatzort;
                    break;
            }

            string msg = token.Value;
            if (token.Identifier == KeywordStreet)
            {
                location.Street = msg;
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
        }

        #endregion
    }
}
