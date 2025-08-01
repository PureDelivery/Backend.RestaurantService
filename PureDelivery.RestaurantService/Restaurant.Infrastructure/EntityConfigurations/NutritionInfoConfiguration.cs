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
    public class NutritionInfoConfiguration : IEntityTypeConfiguration<NutritionInfo>
    {
        public void Configure(EntityTypeBuilder<NutritionInfo> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.MenuItemId)
                .IsRequired();

            builder.Property(n => n.ProteinPer100g)
                .HasPrecision(5, 2);

            builder.Property(n => n.FatPer100g)
                .HasPrecision(5, 2);

            builder.Property(n => n.CarbsPer100g)
                .HasPrecision(5, 2);

            // Enums as strings
            builder.Property(n => n.Allergens)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(n => n.DietaryTags)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.HasIndex(n => n.MenuItemId).IsUnique();
        }
    }
}
