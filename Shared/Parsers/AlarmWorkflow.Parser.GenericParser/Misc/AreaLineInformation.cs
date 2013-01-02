using AlarmWorkflow.Parser.GenericParser.Control;

namespace AlarmWorkflow.Parser.GenericParser.Misc
{
    class AreaLineInformation
    {
        public string AreaString { get; set; }
        public string Value { get; set; }

        public AreaLineInformation(AreaDefinition source, string line)
        {
            Value = GetValueFromLine(source, line);
            AreaString = line.Substring(0, line.Length - Value.Length);

            Value = Value.Trim();
            AreaString = AreaString.Trim();
        }

        public string GetValueFromLine(AreaDefinition source, string line)
        {
            string value = null;

            if (!TryGetValueByAssertingSeparator(source, line, out value))
            {
                TryGetValueByRemovingTheAreaString(source, line, out value);
            }

            return value;
        }

        private bool TryGetValueByAssertingSeparator(AreaDefinition source, string line, out string value)
        {
            value = null;

            int iColon = line.IndexOf(source.Separator);
            if (iColon == -1)
            {
                return false;
            }

            iColon += 1;

            value = line.Remove(0, iColon).Trim();
            return true;
        }

        private bool TryGetValueByRemovingTheAreaString(AreaDefinition source, string line, out string value)
        {
            // Take the area string as the "substitute" and remove the length in hope...
            value = line.Remove(0, source.AreaString.String.Length);

            return true;
        }
    }
}
