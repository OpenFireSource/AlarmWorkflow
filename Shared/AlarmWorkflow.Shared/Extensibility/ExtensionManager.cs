using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Extensibility
{
    /// <summary>
    /// Manages the extensions that are loaded and controls them.
    /// </summary>
    sealed class ExtensionManager : IExtensionHost
    {
        #region Fields

        private List<IExtension> _extensions;
        private List<IExtensionObject> _extensionObjects;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionManager"/> class.
        /// </summary>
        public ExtensionManager()
        {
            _extensions = new List<IExtension>();
            _extensionObjects = new List<IExtensionObject>();

            HostExtensions();
        }

        #endregion

        #region Methods

        private string[] LoadWhitelist()
        {
            string whitelistPath = Utilities.Combine(Utilities.GetWorkingDirectory(Assembly.GetExecutingAssembly()), "Config", "ExtensionWhitelist.lst");
            if (File.Exists(whitelistPath))
            {
                return File.ReadAllLines(whitelistPath);
            }

            return new string[0];
        }

        private void HostExtensions()
        {
            // First collect all extensions without constructing them
            List<ExportedType> extensions = ExportedTypeLibrary.GetExports(typeof(IExtension));

            // Second, check if they are whitelisted
            List<string> whitelistedExtensions = new List<string>(LoadWhitelist());
            foreach (ExportedType exportToCheck in extensions)
            {
                // Make the whitelist-check. It's ok if there is no whitelist, then nothing is allowed (sanity).
                bool isWhitelisted = (whitelistedExtensions.Contains(exportToCheck.Type.FullName));
                if (!isWhitelisted)
                {
                    // If not whitelisted ignore this 
                    continue;
                }

                // Safety-first...
                try
                {
                    IExtension extension = exportToCheck.CreateInstance<IExtension>();
                    extension.Initialize(this);
                    _extensions.Add(extension);
                }
                catch (Exception)
                {
                    // Don't throw an exception here - ignoring this extension is enough
                }
            }
        }

        /// <summary>
        /// Shuts down all extensions.
        /// </summary>
        public void Shutdown()
        {
            _extensions.ForEach(e =>
            {
                try
                {
                    e.Shutdown();
                }
                catch (Exception)
                {

                }
            });
        }

        /// <summary>
        /// Returns all <see cref="IExtensionObject"/> that are of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IExtensionObject"/> to get.</typeparam>
        /// <returns></returns>
        internal IEnumerable<T> GetExtensionsOfType<T>() where T : class,IExtensionObject
        {
            foreach (IExtensionObject eo in _extensionObjects)
            {
                if (eo is T)
                {
                    yield return (T)eo;
                }
            }
        }

        internal T GetExtensionWithName<T>(string name) where T : class,IExtensionObject
        {
            var ext = GetExtensionsOfType<T>();
            if (ext == null)
            {
                return null;
            }

            List<T> extensions = new List<T>(ext);
            return extensions.Find(e => string.Equals(name, e.GetType().Name, StringComparison.InvariantCulture));
        }

        #endregion

        #region IExtensionHost Members

        void IExtensionHost.RegisterJob(IJob job)
        {
            if (!_extensionObjects.Contains(job))
            {
                _extensionObjects.Add(job);
            }
        }

        void IExtensionHost.RegisterParser(IParser parser)
        {
            if (!_extensionObjects.Contains(parser))
            {
                _extensionObjects.Add(parser);
            }
        }

        #endregion
    }
}
