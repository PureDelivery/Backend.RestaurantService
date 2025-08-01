using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.EntityConfigurations
{
    public class ZonePointConfiguration : IEntityTypeConfiguration<ZonePoint>
    {
        public void Configure(EntityTypeBuilder<ZonePoint> builder)
        {
            builder.HasKey(z => z.Id);

            builder.Property(z => z.Latitude)
                .HasPrecision(10, 8)
                .IsRequired();

            builder.Property(z => z.Longitude)
                .HasPrecision(11, 8)
                .IsRequired();

            builder.HasIndex(z => new { z.ZoneId, z.Order }).IsUnique();
        }
    }
}
