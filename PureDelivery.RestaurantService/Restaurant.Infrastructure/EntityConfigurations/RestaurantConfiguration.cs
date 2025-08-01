using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantService.Domain.Entities;
using RestaurantDomain = RestaurantService.Domain.Entities.Restaurant;

namespace Restaurant.Infrastructure.EntityConfigurations
{
    public class RestaurantConfiguration : IEntityTypeConfiguration<RestaurantDomain>
    {
        public void Configure(EntityTypeBuilder<RestaurantDomain> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(r => r.Description)
                .HasMaxLength(1000);

            builder.Property(r => r.ImageUrl)
                .HasMaxLength(500);

            // Enums stored as strings
            builder.Property(r => r.CuisineTypes)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(r => r.Tags)
                .HasConversion<string>()
                .HasMaxLength(50);

            // Indexes
            builder.HasIndex(r => r.Name);
            builder.HasIndex(r => r.IsActive);
            builder.HasIndex(r => r.CuisineTypes);
            builder.HasIndex(r => r.CreatedAt);
        }
    }
}
