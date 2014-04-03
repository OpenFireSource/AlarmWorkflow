// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System.Diagnostics;
using System.IO;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.Backend.ServiceContracts.Communication.EndPointResolvers;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Website.Reports
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Logger.Instance.Initialize("AlarmWorkflow.Website.Mvc");
            SetupWebsiteConfiguration();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void SetupWebsiteConfiguration()
        {
            string awDir = Server.MapPath("~/");

#if DEBUG
            // This code is only used when compiled in DEBUG, and running in Debug mode, to ease development.
            if (Debugger.IsAttached)
            {
                string cfgValue = WebConfigurationManager.AppSettings["Debug.AlarmWorkflowDirectory"];
                if (Directory.Exists(cfgValue))
                {
                    awDir = cfgValue;
                }
            }
#endif

            ServiceFactory.BackendConfigurator = new WebsiteBackendConfigurator(awDir);
            ServiceFactory.EndPointResolver = new LocalhostEndPointResolver();
        }

        class WebsiteBackendConfigurator : IBackendConfigurator
        {
            #region Fields

            private readonly string _targetDirectory;
            private readonly IBackendConfigurator _original;

            #endregion

            #region Constructors

            internal WebsiteBackendConfigurator(string rootDirectory)
            {
                _targetDirectory = rootDirectory;

                string file = Path.Combine(_targetDirectory, "Backend.config");

                _original = new BackendConfigurator(file);
            }

            #endregion

            #region IBackendConfigurator Members

            string IBackendConfigurator.Get(string key)
            {
                switch (key)
                {
                    case "Wcf.ServiceLocations": return Path.Combine(_targetDirectory, "Backend.Services.config");
                    default: return _original.Get(key);
                }
            }

            #endregion
        }
    }
}