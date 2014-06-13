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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using AlarmWorkflow.Backend.ServiceContracts.Communication;
using AlarmWorkflow.BackendService.FileTransferContracts.Client;
using AlarmWorkflow.BackendService.ManagementContracts;
using AlarmWorkflow.BackendService.ManagementContracts.Emk;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Website.Reports.Areas.Display.Models;
using AlarmWorkflow.Website.Reports.Filters;
using Microsoft.Win32;

namespace AlarmWorkflow.Website.Reports.Areas.Display.Controllers
{
    public class AlarmController : Controller
    {
        private Guid _guid = Guid.NewGuid();

        /// <summary>
        /// GET: /Display/Alarm/Index
        /// GET: /Display/Alarm/
        /// </summary>
        /// <returns></returns>
        [CustomHandleError()]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: /Display/Alarm/GetLatestOperation
        /// </summary>
        /// <returns></returns>
        public ActionResult GetLatestOperation()
        {
            GetLastOperationData data = new GetLastOperationData();
            try
            {
                using (var service = ServiceFactory.GetCallbackServiceWrapper<IOperationService>(new OperationServiceCallback()))
                {
                    IList<int> ids = service.Instance.GetOperationIds(WebsiteConfiguration.Instance.MaxAge, WebsiteConfiguration.Instance.NonAcknowledgedOnly, 1);
                    if (ids.Count == 1)
                    {
                        Operation item = service.Instance.GetOperationById(ids[0]);
                        data.success = true;
                        data.op = item;
                    }
                    else if (ids.Count == 0)
                    {
                        data.success = true;
                        data.op = null;
                    }
                }
            }
            catch (Exception ex)
            {
                // It's ok when an exception is thrown here. We catch it, log it, and the View considers it as an error (success is false).
                Logger.Instance.LogException(this, ex);
            }

            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            result.Data = data;
            return result;
        }

        /// <summary>
        /// GET: /Display/Alarm/ResetOperation/Id
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetOperation(int id)
        {
            ResetOperationData returnValue = new ResetOperationData();
            try
            {
                using (var service = ServiceFactory.GetCallbackServiceWrapper<IOperationService>(new OperationServiceCallback()))
                {
                    Operation item = service.Instance.GetOperationById(id);
                    if (item == null)
                    {
                        returnValue.success = false;
                        returnValue.message = "Operation not found!";
                    }
                    else if (item.IsAcknowledged)
                    {
                        returnValue.success = false;
                        returnValue.message = "Operation is already acknowledged!";
                    }
                    else
                    {
                        service.Instance.AcknowledgeOperation(id);
                        returnValue.success = true;
                        returnValue.message = "Operation successfully acknowledged!";
                    }
                }
            }
            catch (Exception ex)
            {
                // It's ok when an exception is thrown here. We catch it, log it, and the View considers it as an error (success is false).
                Logger.Instance.LogException(this, ex);
            }
            JsonResult jsonResult = new JsonResult();
            jsonResult.Data = returnValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        /// <summary>
        /// GET: /Display/Alarm/ResetLatestOperation
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetLatestOperation()
        {
            JsonResult result = GetLatestOperation() as JsonResult;
            if (result != null)
            {
                GetLastOperationData data = result.Data as GetLastOperationData;
                if (data != null && (data.success && data.op != null))
                {
                    return ResetOperation(data.op.Id);
                }

            }
            JsonResult jsonResult = new JsonResult();
            jsonResult.Data = new ResetOperationData { message = "An undefined error occured.", success = false };
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        /// <summary>
        /// Gets the list of filtered resources for a given operation
        /// GET: /Display/Alarm/GetFilteredResources/Id
        /// </summary>
        /// <param name="id">The id of the operation</param>
        /// <returns>A <see cref="JsonResult"/> containing a <see cref="ResourcesData"/> object or null if an error occured.</returns>
        public ActionResult GetFilteredResources(int id)
        {
            using (var emkService = ServiceFactory.GetServiceWrapper<IEmkService>())
            {
                Operation operation;
                try
                {
                    using (var service = ServiceFactory.GetCallbackServiceWrapper<IOperationService>(new OperationServiceCallback()))
                    {
                        operation = service.Instance.GetOperationById(id);
                        if (operation == null)
                        {
                            return null;
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }

                ResourcesData data = new ResourcesData();
                IList<EmkResource> emkResources = emkService.Instance.GetAllResources();
                List<ResourceObject> filteredResources = new List<ResourceObject>();

                IList<OperationResource> filtered = emkService.Instance.GetFilteredResources(operation.Resources);
                foreach (OperationResource resource in filtered)
                {
                    EmkResource emk = emkResources.FirstOrDefault(item => item.IsActive && item.IsMatch(resource));
                    filteredResources.Add(new ResourceObject(emk, resource));
                }
                data.Resources = filteredResources;

                JsonResult result = new JsonResult();
                result.Data = data;
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }

        /// <summary>
        /// Gets the image assigned to a filtered resource.
        /// The image gets cached on the client (and the server) for 2 minutes.
        /// GET: /Display/Alarm/GetResourceImage/Id
        /// </summary>
        /// <param name="id">The id of the resource.</param>
        /// <returns>The image (<see cref="File"/>) or null if the image was not found.  </returns>
        [OutputCache(Duration = 120, VaryByParam = "id", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult GetResourceImage(string id)
        {
            using (var emkService = ServiceFactory.GetServiceWrapper<IEmkService>())
            {
                IList<EmkResource> emkResources = emkService.Instance.GetAllResources();
                EmkResource resource = emkResources.FirstOrDefault(x => x.Id == id);
                if (resource == null)
                {
                    return null;
                }
                using (FileTransferServiceClient client = new FileTransferServiceClient())
                {
                    try
                    {
                        Stream content = client.GetFileFromPath(resource.IconFileName);

                        if (content != null)
                        {
                            return File(content, GetMimeType(resource.IconFileName));
                        }
                    }
                    catch (IOException ex)
                    {
                        // This exception is totally OK. No image will be displayed.
                    }
                    catch (AssertionFailedException)
                    {
                        // This exception is totally OK. No image will be displayed.
                    }
                }
            }
            return null;
        }

        private static string GetMimeType(string path)
        {
            const string unkownMimeType = "application/unknown";
            RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(Path.GetExtension(path).ToLower());

            if (regKey == null)
            {
                return unkownMimeType;
            }

            object contentType = regKey.GetValue("Content Type");

            return (contentType == null) ? unkownMimeType : contentType.ToString();
        }

    }
}
