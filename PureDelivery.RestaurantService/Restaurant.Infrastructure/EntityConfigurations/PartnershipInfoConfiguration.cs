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
    public class PartnershipInfoConfiguration : IEntityTypeConfiguration<PartnershipInfo>
    {
        public void Configure(EntityTypeBuilder<PartnershipInfo> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.CommissionRate)
                .HasPrecision(5, 4)
                .IsRequired();

            builder.Property(p => p.ContractNumber)
                .HasMaxLength(50);

            builder.HasIndex(p => p.IsActive);
            builder.HasIndex(p => p.ContractNumber).IsUnique();
        }
    }
}
