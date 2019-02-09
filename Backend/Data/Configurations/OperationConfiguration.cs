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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlarmWorkflow.Backend.Data.Configurations
{
    // ReSharper disable once UnusedMember.Global
    class OperationConfiguration : ConfigurationBase<OperationData>
    {
        #region Methods

        public override void Configure(EntityTypeBuilder<OperationData> builder)
        {
            base.Configure(builder);

            builder.Property(o => o.IsAcknowledged).HasColumnName("acknowledged").IsRequired();
            builder.Property(o => o.Guid).HasColumnName("operationguid").IsRequired();
            builder.Property(o => o.OperationNumber).HasColumnName("operationnumber");
            builder.Property(o => o.IncomeAt).HasColumnName("timestampincome");
            builder.Property(o => o.AlarmAt).HasColumnName("timestampalarm");
            builder.Property(o => o.Messenger).HasColumnName("messenger");
            builder.Property(o => o.Comment).HasColumnName("comment");
            builder.Property(o => o.Plan).HasColumnName("plan");
            builder.Property(o => o.Picture).HasColumnName("picture");
            builder.Property(o => o.Priority).HasColumnName("priority");

            builder.OwnsOne(o => o.Einsatzort).Property(o => o.Street).HasColumnName("einsatzortstreet");
            builder.OwnsOne(o => o.Einsatzort).Property(o => o.StreetNumber).HasColumnName("einsatzortstreetnumber");
            builder.OwnsOne(o => o.Einsatzort).Property(o => o.City).HasColumnName("einsatzortcity");
            builder.OwnsOne(o => o.Einsatzort).Property(o => o.ZipCode).HasColumnName("einsatzortzipcode");
            builder.OwnsOne(o => o.Einsatzort).Property(o => o.Intersection).HasColumnName("einsatzortintersection");
            builder.OwnsOne(o => o.Einsatzort).Property(o => o.Property).HasColumnName("einsatzortproperty");
            builder.OwnsOne(o => o.Einsatzort).Property(o => o.Location).HasColumnName("einsatzortlocation");
            builder.OwnsOne(o => o.Einsatzort).Property(o => o.GeoLatLng).HasColumnName("einsatzortlatlng");

            builder.OwnsOne(o => o.Zielort).Property(o => o.Street).HasColumnName("zielortstreet");
            builder.OwnsOne(o => o.Zielort).Property(o => o.StreetNumber).HasColumnName("zielortstreetnumber");
            builder.OwnsOne(o => o.Zielort).Property(o => o.City).HasColumnName("zielortcity");
            builder.OwnsOne(o => o.Zielort).Property(o => o.ZipCode).HasColumnName("zielortzipcode");
            builder.OwnsOne(o => o.Zielort).Property(o => o.Intersection).HasColumnName("zielortintersection");
            builder.OwnsOne(o => o.Zielort).Property(o => o.Property).HasColumnName("zielortproperty");
            builder.OwnsOne(o => o.Zielort).Property(o => o.Location).HasColumnName("zielortlocation");
            builder.OwnsOne(o => o.Zielort).Property(o => o.GeoLatLng).HasColumnName("zielortlatlng");

            builder.OwnsOne(o => o.Keywords).Property(o => o.Keyword).HasColumnName("keyword");
            builder.OwnsOne(o => o.Keywords).Property(o => o.EmergencyKeyword).HasColumnName("keywordmisc");
            builder.OwnsOne(o => o.Keywords).Property(o => o.B).HasColumnName("keywordb");
            builder.OwnsOne(o => o.Keywords).Property(o => o.R).HasColumnName("keywordr");
            builder.OwnsOne(o => o.Keywords).Property(o => o.S).HasColumnName("keywords");
            builder.OwnsOne(o => o.Keywords).Property(o => o.T).HasColumnName("keywordt");

            builder.Property(o => o.Loops).HasColumnName("loopscsv");
            builder.Property(o => o.CustomData).HasColumnName("customdatajson");

            builder.HasMany(o => o.Resources).WithOne(r => r.Operation).IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.DispositionedResources).WithOne(r => r.Operation).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }


        protected override string GetTableName()
        {
            return "operation";
        }

        #endregion
    }
}
