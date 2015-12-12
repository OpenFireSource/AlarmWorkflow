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

using AlarmWorkflow.Backend.Data.Types;

namespace AlarmWorkflow.Backend.Data.Configurations
{
    class OperationConfiguration : ConfigurationBase<OperationData>
    {
        #region Constructors

        public OperationConfiguration()
        {
            Property(o => o.IsAcknowledged).HasColumnName("acknowledged").IsRequired();
            Property(o => o.Guid).HasColumnName("operationguid").IsRequired();
            Property(o => o.OperationNumber).HasColumnName("operationnumber");
            Property(o => o.IncomeAt).HasColumnName("timestampincome");
            Property(o => o.AlarmAt).HasColumnName("timestampalarm");
            Property(o => o.Messenger).HasColumnName("messenger");
            Property(o => o.Comment).HasColumnName("comment");
            Property(o => o.Plan).HasColumnName("plan");
            Property(o => o.Picture).HasColumnName("picture");
            Property(o => o.Priority).HasColumnName("priority");

            Property(o => o.Einsatzort.Street).HasColumnName("einsatzortstreet");
            Property(o => o.Einsatzort.StreetNumber).HasColumnName("einsatzortstreetnumber");
            Property(o => o.Einsatzort.City).HasColumnName("einsatzortcity");
            Property(o => o.Einsatzort.ZipCode).HasColumnName("einsatzortzipcode");
            Property(o => o.Einsatzort.Intersection).HasColumnName("einsatzortintersection");
            Property(o => o.Einsatzort.Property).HasColumnName("einsatzortproperty");
            Property(o => o.Einsatzort.Location).HasColumnName("einsatzortlocation");
            Property(o => o.Einsatzort.GeoLatLng).HasColumnName("einsatzortlatlng");

            Property(o => o.Zielort.Street).HasColumnName("zielortstreet");
            Property(o => o.Zielort.StreetNumber).HasColumnName("zielortstreetnumber");
            Property(o => o.Zielort.City).HasColumnName("zielortcity");
            Property(o => o.Zielort.ZipCode).HasColumnName("zielortzipcode");
            Property(o => o.Zielort.Intersection).HasColumnName("zielortintersection");
            Property(o => o.Zielort.Property).HasColumnName("zielortproperty");
            Property(o => o.Zielort.Location).HasColumnName("zielortlocation");
            Property(o => o.Zielort.GeoLatLng).HasColumnName("zielortlatlng");

            Property(o => o.Keywords.Keyword).HasColumnName("keyword");
            Property(o => o.Keywords.EmergencyKeyword).HasColumnName("keywordmisc");
            Property(o => o.Keywords.B).HasColumnName("keywordb");
            Property(o => o.Keywords.R).HasColumnName("keywordr");
            Property(o => o.Keywords.S).HasColumnName("keywords");
            Property(o => o.Keywords.T).HasColumnName("keywordt");

            Property(o => o.Loops).HasColumnName("loopscsv");
            Property(o => o.CustomData).HasColumnName("customdatajson");

            HasMany(o => o.Resources)
                .WithRequired(r => r.Operation)
                .HasForeignKey(r => r.OperationId)
                .WillCascadeOnDelete();

            HasMany(o => o.DispositionedResources)
                .WithRequired(dr => dr.Operation)
                .HasForeignKey(dr => dr.OperationId)
                .WillCascadeOnDelete();
        }

        #endregion

        #region Methods

        protected override string GetTableName()
        {
            return "operation";
        }

        #endregion
    }
}
