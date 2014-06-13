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
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.Management.Data
{
    partial class OperationData
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationData"/> class.
        /// </summary>
        public OperationData()
        {
            this.OperationGuid = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationData"/> class,
        /// and copies the contents of the given <see cref="Operation"/> to this entity.
        /// </summary>
        /// <param name="operation">The operation to copy its contents.</param>
        public OperationData(Operation operation)
            : this()
        {
            this.IsAcknowledged = operation.IsAcknowledged;
            this.OperationNumber = operation.OperationNumber;
            this.TimestampIncome = operation.TimestampIncome;
            this.TimestampAlarm = operation.Timestamp;
            this.Comment = operation.Comment;
            this.Messenger = operation.Messenger;
            this.Plan = operation.OperationPlan;
            this.Picture = operation.Picture;
            this.Priority = operation.Priority;
            this.LoopsCsv = operation.Loops.ToString();

            if (operation.Einsatzort != null)
            {
                this.EinsatzortStreet = operation.Einsatzort.Street;
                this.EinsatzortStreetNumber = operation.Einsatzort.StreetNumber;
                this.EinsatzortZipCode = operation.Einsatzort.ZipCode;
                this.EinsatzortCity = operation.Einsatzort.City;
                this.EinsatzortIntersection = operation.Einsatzort.Intersection;
                this.EinsatzortLocation = operation.Einsatzort.Location;
                this.EinsatzortProperty = operation.Einsatzort.Property;
                this.EinsatzortLatLng = string.Format("{0};{1}", operation.Einsatzort.GeoLatitude, operation.Einsatzort.GeoLongitude);
            }
            if (operation.Zielort != null)
            {
                this.ZielortStreet = operation.Zielort.Street;
                this.ZielortStreetNumber = operation.Zielort.StreetNumber;
                this.ZielortZipCode = operation.Zielort.ZipCode;
                this.ZielortCity = operation.Zielort.City;
                this.ZielortIntersection = operation.Zielort.Intersection;
                this.ZielortLocation = operation.Zielort.Location;
                this.ZielortProperty = operation.Zielort.Property;
                this.ZielortLatLng = string.Format("{0};{1}", operation.Zielort.GeoLatitude, operation.Zielort.GeoLongitude);
            }
            if (operation.Keywords != null)
            {
                this.Keyword = operation.Keywords.Keyword;
                this.KeywordMisc = operation.Keywords.EmergencyKeyword;
                this.KeywordB = operation.Keywords.B;
                this.KeywordR = operation.Keywords.R;
                this.KeywordS = operation.Keywords.S;
                this.KeywordT = operation.Keywords.T;
            }

            this.CustomDataJson = JsonHelper.ToJson(operation.CustomData);

            foreach (OperationResource item in operation.Resources)
            {
                this.OperationResources.Add(new OperationResourceData(item));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Copies the contents of this entity to a new instance of the <see cref="Operation"/> class.
        /// </summary>
        /// <returns></returns>
        public Operation ToOperation()
        {
            Operation operation = new Operation()
            {
                Id = this.Id,
                IsAcknowledged = this.IsAcknowledged,
                OperationGuid = this.OperationGuid,
                OperationNumber = this.OperationNumber,
                Timestamp = this.TimestampAlarm,
                TimestampIncome = this.TimestampIncome,
                OperationPlan = this.Plan,
                Comment = this.Comment,
                Messenger = this.Messenger,
                Picture = this.Picture,
                Priority = this.Priority,
                Einsatzort = GetEinsatzort(this),
                Zielort = GetZielort(this),
                Keywords = new OperationKeywords()
                {
                    Keyword = this.Keyword,
                    EmergencyKeyword = this.KeywordMisc,
                    B = this.KeywordB,
                    R = this.KeywordR,
                    S = this.KeywordS,
                    T = this.KeywordT,
                },
                Loops = new OperationLoopCollection(this.LoopsCsv),
                CustomData = JsonHelper.FromJson<IDictionary<string, object>>(this.CustomDataJson, new Dictionary<string, object>()),
            };
            foreach (OperationResourceData item in this.OperationResources)
            {
                operation.Resources.Add(item.ToOperationResource());
            }

            return operation;
        }

        private PropertyLocation GetEinsatzort(OperationData entity)
        {
            PropertyLocation pl = new PropertyLocation();
            pl.Street = entity.EinsatzortStreet;
            pl.StreetNumber = entity.EinsatzortStreetNumber;
            pl.ZipCode = entity.EinsatzortZipCode;
            pl.City = entity.EinsatzortCity;
            pl.Intersection = entity.EinsatzortIntersection;
            pl.Location = entity.EinsatzortLocation;
            pl.Property = entity.EinsatzortProperty;

            string[] latlng = entity.EinsatzortLatLng.Split(';');
            pl.GeoLatitude = latlng[0];
            pl.GeoLongitude = latlng[1];

            return pl;
        }

        private PropertyLocation GetZielort(OperationData entity)
        {
            PropertyLocation pl = new PropertyLocation();
            pl.Street = entity.ZielortStreet;
            pl.StreetNumber = entity.ZielortStreetNumber;
            pl.ZipCode = entity.ZielortZipCode;
            pl.City = entity.ZielortCity;
            pl.Intersection = entity.ZielortIntersection;
            pl.Location = entity.ZielortLocation;
            pl.Property = entity.ZielortProperty;

            string[] latlng = entity.ZielortLatLng.Split(';');
            pl.GeoLatitude = latlng[0];
            pl.GeoLongitude = latlng[1];

            return pl;
        }

        #endregion
    }
}