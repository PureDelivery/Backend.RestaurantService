using Microsoft.EntityFrameworkCore;
using Restaurant.Infrastructure.EntityConfigurations;
using RestaurantService.Domain.Entities;
using System.Collections.Generic;
using System.Net;
using System.Reflection.Emit;
using RestaurantDomain = RestaurantService.Domain.Entities.Restaurant;

namespace RestaurantService.Infrastructure.Data
{
    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options)
        {
        }

        // DbSets для всех таблиц
        public DbSet<RestaurantDomain> Restaurants { get; set; } = null!;
        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<RestaurantSettings> RestaurantSettings { get; set; } = null!;
        public DbSet<PartnershipInfo> PartnershipInfos { get; set; } = null!;
        public DbSet<RestaurantMarketing> RestaurantMarketings { get; set; } = null!;
        public DbSet<RestaurantReview> RestaurantReviews { get; set; } = null!;
        public DbSet<WorkingHours> WorkingHours { get; set; } = null!;
        public DbSet<DeliveryZone> DeliveryZones { get; set; } = null!;
        public DbSet<ZonePoint> ZonePoints { get; set; } = null!;
        public DbSet<MenuItem> MenuItems { get; set; } = null!;
        public DbSet<NutritionInfo> NutritionInfos { get; set; } = null!;
        public DbSet<MenuItemPopularity> MenuItemPopularities { get; set; } = null!;
        public DbSet<MenuItemOption> MenuItemOptions { get; set; } = null!;
        public DbSet<OptionChoice> OptionChoices { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations
            ApplyEntityConfigurations(modelBuilder);

            // Configure relationships
            ConfigureRelationships(modelBuilder);
        }

        private static void ApplyEntityConfigurations(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RestaurantConfiguration());
            modelBuilder.ApplyConfiguration(new AddressConfiguration());
            modelBuilder.ApplyConfiguration(new RestaurantSettingsConfiguration());
            modelBuilder.ApplyConfiguration(new PartnershipInfoConfiguration());
            modelBuilder.ApplyConfiguration(new RestaurantMarketingConfiguration());
            modelBuilder.ApplyConfiguration(new RestaurantReviewConfiguration());
            modelBuilder.ApplyConfiguration(new WorkingHoursConfiguration());
            modelBuilder.ApplyConfiguration(new DeliveryZoneConfiguration());
            modelBuilder.ApplyConfiguration(new ZonePointConfiguration());
            modelBuilder.ApplyConfiguration(new MenuItemConfiguration());
            modelBuilder.ApplyConfiguration(new NutritionInfoConfiguration());
            modelBuilder.ApplyConfiguration(new MenuItemPopularityConfiguration());
            modelBuilder.ApplyConfiguration(new MenuItemOptionConfiguration());
            modelBuilder.ApplyConfiguration(new OptionChoiceConfiguration());
        }

        private static void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            ConfigureRestaurantRelationships(modelBuilder);
            ConfigureMenuRelationships(modelBuilder);
            ConfigureDeliveryRelationships(modelBuilder);
        }

        private static void ConfigureRestaurantRelationships(ModelBuilder modelBuilder)
        {
            // Restaurant -> Address (One-to-One)
            modelBuilder.Entity<RestaurantDomain>()
                .HasOne(r => r.Address)
                .WithOne(a => a.Restaurant)
                .HasForeignKey<Address>(a => a.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Restaurant -> Settings (One-to-One)
            modelBuilder.Entity<RestaurantDomain>()
                .HasOne(r => r.Settings)
                .WithOne(s => s.Restaurant)
                .HasForeignKey<RestaurantSettings>(s => s.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Restaurant -> Partnership (One-to-One)
            modelBuilder.Entity<RestaurantDomain>()
                .HasOne(r => r.Partnership)
                .WithOne(p => p.Restaurant)
                .HasForeignKey<PartnershipInfo>(p => p.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Restaurant -> Marketing (One-to-One)
            modelBuilder.Entity<RestaurantDomain>()
                .HasOne(r => r.Marketing)
                .WithOne(m => m.Restaurant)
                .HasForeignKey<RestaurantMarketing>(m => m.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Restaurant -> Reviews (One-to-Many)
            modelBuilder.Entity<RestaurantDomain>()
                .HasMany(r => r.Reviews)
                .WithOne(rv => rv.Restaurant)
                .HasForeignKey(rv => rv.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Restaurant -> WorkingHours (One-to-Many)
            modelBuilder.Entity<RestaurantDomain>()
                .HasMany(r => r.WorkingHours)
                .WithOne(wh => wh.Restaurant)
                .HasForeignKey(wh => wh.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void ConfigureMenuRelationships(ModelBuilder modelBuilder)
        {
            // Restaurant -> MenuItems (One-to-Many)
            modelBuilder.Entity<RestaurantDomain>()
                .HasMany(r => r.MenuItems)
                .WithOne(mi => mi.Restaurant)
                .HasForeignKey(mi => mi.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            // MenuItem -> NutritionInfo (One-to-One)
            modelBuilder.Entity<MenuItem>()
                .HasOne(mi => mi.Nutrition)
                .WithOne(ni => ni.MenuItem)
                .HasForeignKey<NutritionInfo>(ni => ni.MenuItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // MenuItem -> Popularity (One-to-One)
            modelBuilder.Entity<MenuItem>()
                .HasOne(mi => mi.Popularity)
                .WithOne(p => p.MenuItem)
                .HasForeignKey<MenuItemPopularity>(p => p.MenuItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // MenuItem -> Options (One-to-Many)
            modelBuilder.Entity<MenuItem>()
                .HasMany(mi => mi.Options)
                .WithOne(opt => opt.MenuItem)
                .HasForeignKey(opt => opt.MenuItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // MenuItemOption -> OptionChoices (One-to-Many)
            modelBuilder.Entity<MenuItemOption>()
                .HasMany(opt => opt.Choices)
                .WithOne(ch => ch.Option)
                .HasForeignKey(ch => ch.OptionId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void ConfigureDeliveryRelationships(ModelBuilder modelBuilder)
        {
            // Restaurant -> DeliveryZones (One-to-Many)
            modelBuilder.Entity<RestaurantDomain>()
                .HasMany(r => r.DeliveryZones)
                .WithOne(dz => dz.Restaurant)
                .HasForeignKey(dz => dz.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            // DeliveryZone -> ZonePoints (One-to-Many)
            modelBuilder.Entity<DeliveryZone>()
                .HasMany(dz => dz.Points)
                .WithOne(zp => zp.Zone)
                .HasForeignKey(zp => zp.ZoneId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}