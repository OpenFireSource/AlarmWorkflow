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
using System.Web.Mvc;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Website.Reports.Filters
{
    /// <summary>
    /// Represents an attribute that derives from the <see cref="HandleErrorAttribute"/> and additionally logs all exceptions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        #region Methods

        /// <summary>
        /// Overridden to log every error, too.
        /// </summary>
        /// <param name="filterContext">The filter context to use.</param>
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            Logger.Instance.LogException(filterContext.Controller, filterContext.Exception);
        }

        #endregion
    }
}