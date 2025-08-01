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
    public class OptionChoiceConfiguration : IEntityTypeConfiguration<OptionChoice>
    {
        public void Configure(EntityTypeBuilder<OptionChoice> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.PriceModifier)
                .HasPrecision(8, 2);

            builder.HasIndex(c => c.OptionId);
        }
    }
}
