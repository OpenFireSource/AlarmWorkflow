﻿// This file is part of AlarmWorkflow.
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
    class DispositionedResourceConfiguration : ConfigurationBase<DispositionedResourceData>
    {
        #region Methods

        public override void Configure(EntityTypeBuilder<DispositionedResourceData> builder)
        {
            base.Configure(builder);

            builder.Property(dr => dr.OperationId).HasColumnName("operation_id").IsRequired();
            builder.Property(dr => dr.Timestamp).HasColumnName("timestamp").IsRequired();
            builder.Property(dr => dr.EmkResourceId).HasColumnName("emkresourceid").IsRequired();
        }

        protected override string GetTableName()
        {
            return "dispresource";
        }

        #endregion
    }
}
