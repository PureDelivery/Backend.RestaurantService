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
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.FullAddress)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(a => a.City)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(a => a.District)
                .HasMaxLength(100);

            builder.Property(a => a.Street)
                .HasMaxLength(200);

            builder.Property(a => a.Building)
                .HasMaxLength(50);

            builder.Property(a => a.PostalCode)
                .HasMaxLength(20);

            builder.Property(a => a.Latitude)
                .HasPrecision(10, 8)
                .IsRequired();

            builder.Property(a => a.Longitude)
                .HasPrecision(11, 8)
                .IsRequired();

            builder.Property(a => a.Landmark)
                .HasMaxLength(200);

            builder.Property(a => a.AccessInstructions)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(a => new { a.Latitude, a.Longitude });

            builder.HasIndex(a => a.City);
        }
    }

}
