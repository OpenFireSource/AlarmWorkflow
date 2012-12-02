using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlarmWorkflow.Tools.PluginManager
{
    public class Plugin
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Url { get; private set; }
        /// <summary>
        /// The news Version
        /// </summary>
        public string version { get; private set; }
        /// <summary>
        /// The Version that is downloaded
        /// </summary>
        public string versionnow { get; private set; }
        public bool IsDownloaded { get; private set; }
        private string Category { get; set; }
        private string filename { get; set; }

        public Plugin(string ID, string NAME, string DESCRIPTION, string URL, string VERSION, string VERSIONNOW, bool ISDOWNLOADED, string CATEGORY, string FILENAME)
        {
            Id = ID;
            Name = NAME;
            Description = DESCRIPTION;
            Url = URL;
            version = VERSION;
            versionnow = VERSIONNOW;
            IsDownloaded = ISDOWNLOADED;
            Category = CATEGORY;
            filename = FILENAME;
        }
    }
}
