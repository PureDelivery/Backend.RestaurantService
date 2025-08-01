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
    public class MenuItemPopularityConfiguration : IEntityTypeConfiguration<MenuItemPopularity>
    {
        public void Configure(EntityTypeBuilder<MenuItemPopularity> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasIndex(p => p.IsPopular);
            builder.HasIndex(p => p.IsRecommended);
            builder.HasIndex(p => p.IsNew);
            builder.HasIndex(p => p.MenuItemId).IsUnique();
        }
    }
}
