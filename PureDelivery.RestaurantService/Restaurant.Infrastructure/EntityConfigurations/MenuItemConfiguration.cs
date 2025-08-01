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
    public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
    {
        public void Configure(EntityTypeBuilder<MenuItem> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(m => m.Description)
                .HasMaxLength(1000);

            builder.Property(m => m.Price)
                .HasPrecision(8, 2)
                .IsRequired();

            builder.Property(m => m.Category)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(m => m.ImageUrl)
                .HasMaxLength(500);

            builder.Property(m => m.UnavailabilityReason)
                .HasMaxLength(200);

            builder.HasIndex(m => m.RestaurantId);
            builder.HasIndex(m => m.Category);
            builder.HasIndex(m => m.IsAvailable);
            builder.HasIndex(m => new { m.RestaurantId, m.Category });
        }
    }
}
