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
    public class RestaurantMarketingConfiguration : IEntityTypeConfiguration<RestaurantMarketing>
    {
        public void Configure(EntityTypeBuilder<RestaurantMarketing> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.AccountManagerName)
                .HasMaxLength(100);

            builder.Property(m => m.AccountManagerEmail)
                .HasMaxLength(100);

            builder.HasIndex(m => m.IsFeatured);
            builder.HasIndex(m => m.SearchPriority);
        }
    }
}
