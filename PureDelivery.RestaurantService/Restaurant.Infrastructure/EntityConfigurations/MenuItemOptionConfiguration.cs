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
    public class MenuItemOptionConfiguration : IEntityTypeConfiguration<MenuItemOption>
    {
        public void Configure(EntityTypeBuilder<MenuItemOption> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.MenuItemId)
                .IsRequired();

            builder.Property(o => o.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(o => o.Type)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(o => o.MenuItemId);
        }
    }
}