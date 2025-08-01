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
    public class DeliveryZoneConfiguration : IEntityTypeConfiguration<DeliveryZone>
    {
        public void Configure(EntityTypeBuilder<DeliveryZone> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(d => d.DeliveryFee)
                .HasPrecision(8, 2)
                .IsRequired();

            builder.Property(d => d.MinOrderAmount)
                .HasPrecision(8, 2)
                .IsRequired();

            builder.HasIndex(d => d.RestaurantId);
            builder.HasIndex(d => d.IsActive);
            builder.HasIndex(d => d.Priority);
        }
    }
}
