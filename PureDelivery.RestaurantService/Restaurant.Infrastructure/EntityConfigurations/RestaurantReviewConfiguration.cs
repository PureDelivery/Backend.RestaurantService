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
    public class RestaurantReviewConfiguration : IEntityTypeConfiguration<RestaurantReview>
    {
        public void Configure(EntityTypeBuilder<RestaurantReview> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Rating)
                .IsRequired();

            builder.Property(r => r.Comment)
                .HasMaxLength(1000);

            builder.HasIndex(r => r.RestaurantId);
            builder.HasIndex(r => r.CustomerId);
            builder.HasIndex(r => r.Rating);
            builder.HasIndex(r => r.CreatedAt);
        }
    }
}
