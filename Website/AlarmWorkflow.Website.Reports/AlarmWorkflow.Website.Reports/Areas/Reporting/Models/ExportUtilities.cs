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
using System.Linq.Expressions;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Website.Reports.Areas.Reporting.Models
{
    class ExportUtilities
    {
        internal static Stream ExportOperation(Operation operation)
        {
            MemoryStream stream = new MemoryStream();
            
            XDocument doc = new XDocument();
            doc.Add(new XElement("AlarmWorkflowOperation"));
            doc.Root.Add(CreateXElement("Exported", DateTime.UtcNow));
            doc.Root.Add(CreateXElement(() => operation.Id));
            doc.Root.Add(CreateXElement(() => operation.OperationGuid));
            doc.Root.Add(CreateXElement(() => operation.TimestampIncome));
            doc.Root.Add(CreateXElement(() => operation.Timestamp));
            doc.Root.Add(CreateXElement(() => operation.Messenger));
            doc.Root.Add(CreateXElement(() => operation.Comment));
            doc.Root.Add(CreateXElement(() => operation.OperationPlan));
            doc.Root.Add(CreateXElement(() => operation.Picture));
            doc.Root.Add(CreateXElement(() => operation.Priority));

            doc.Root.Add(WritePropertyLocation("Einsatzort", operation.Einsatzort));
            doc.Root.Add(WritePropertyLocation("Zielort", operation.Zielort));

            XElement keywordsE = new XElement("Keywords");
            keywordsE.Add(CreateXElement(() => operation.Keywords.Keyword));
            keywordsE.Add(CreateXElement(() => operation.Keywords.EmergencyKeyword));
            keywordsE.Add(CreateXElement(() => operation.Keywords.B));
            keywordsE.Add(CreateXElement(() => operation.Keywords.R));
            keywordsE.Add(CreateXElement(() => operation.Keywords.S));
            keywordsE.Add(CreateXElement(() => operation.Keywords.T));
            doc.Root.Add(keywordsE);

            XElement loopsE = new XElement("Loops");
            foreach (string loop in operation.Loops)
            {
                loopsE.Add(new XElement("Loop", loop));
            }
            doc.Root.Add(loopsE);

            XElement resourcesE = new XElement("Resources");
            foreach (OperationResource resource in operation.Resources)
            {
                XElement resourceE = new XElement("Resource");
                resourceE.Add(CreateXElement(() => resource.FullName));
                resourceE.Add(CreateXElement(() => resource.Timestamp));
                foreach (string equip in resource.RequestedEquipment)
                {
                    resourceE.Add(CreateXElement("Equipment", equip));
                }

                resourcesE.Add(resourceE);
            }
            doc.Root.Add(resourcesE);

            XElement customDataE = new XElement("CustomData");
            foreach (KeyValuePair<string, object> pair in operation.CustomData)
            {
                XElement entry = new XElement("Entry");
                entry.Add(new XAttribute("Key", pair.Key));
                entry.Add(new XAttribute("Value", pair.Value));
                customDataE.Add(entry);
            }
            doc.Root.Add(customDataE);

            doc.Save(stream);

            stream.Position = 0L;
            return stream;
        }

        private static XElement WritePropertyLocation(string name, PropertyLocation propertyLocation)
        {
            XElement element = new XElement(name);
            element.Add(CreateXElement(() => propertyLocation.Street));
            element.Add(CreateXElement(() => propertyLocation.StreetNumber));
            element.Add(CreateXElement(() => propertyLocation.ZipCode));
            element.Add(CreateXElement(() => propertyLocation.City));
            element.Add(CreateXElement(() => propertyLocation.Intersection));
            element.Add(CreateXElement(() => propertyLocation.Property));
            element.Add(CreateXElement(() => propertyLocation.GeoLatitude));
            element.Add(CreateXElement(() => propertyLocation.GeoLongitude));
            return element;
        }

        private static XElement CreateXElement<T>(Expression<Func<T>> propertyExpression)
        {
            MemberExpression me = propertyExpression.Body as MemberExpression;
            if (me == null)
            {
                throw new InvalidOperationException("Invalid expression!");
            }

            string name = me.Member.Name;
            object value = propertyExpression.Compile().Invoke();

            return CreateXElement(name, value);
        }

        private static XElement CreateXElement(string name, object value)
        {
            string v = (value != null) ? value.ToString() : string.Empty;
            return new XElement(name, v);
        }
    }
}