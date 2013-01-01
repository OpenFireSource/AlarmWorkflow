using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AlarmWorkflow.Parser.GenericParser.Parsing
{
    static class SectionParserCache
    {
        private static readonly IEnumerable<Type> _types;

        public static IEnumerable<Type> Types
        {
            get { return SectionParserCache._types; }
        }

        static SectionParserCache()
        {
            _types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetInterface(typeof(ISectionParser).Name) != null);
        }

        public static ISectionParser Create(string name)
        {
            Type type = _types.FirstOrDefault(t => t.Name == name);
            return (ISectionParser)Activator.CreateInstance(type);
        }

    }
}
