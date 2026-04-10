using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantService.Domain.Entities;

namespace Restaurant.Infrastructure.EntityConfigurations
{
    public class RestaurantManagerConfiguration : IEntityTypeConfiguration<RestaurantManager>
    {
        public void Configure(EntityTypeBuilder<RestaurantManager> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.UserId).IsRequired();
            builder.Property(e => e.RestaurantId).IsRequired();

            builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Phone).HasMaxLength(20);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(256);

            builder.Property(e => e.IsOwner).IsRequired().HasDefaultValue(false);
            builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            builder.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(e => e.UserId).IsUnique().HasDatabaseName("RestaurantManagers_UserId");
            builder.HasIndex(e => e.Email).IsUnique().HasDatabaseName("RestaurantManagers_Email");
            builder.HasIndex(e => e.RestaurantId).HasDatabaseName("RestaurantManagers_RestaurantId");

            builder.ToTable("RestaurantManagers");
        }
    }
}
