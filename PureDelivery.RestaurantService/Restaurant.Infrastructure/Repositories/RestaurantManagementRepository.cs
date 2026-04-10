using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Repositories;
using RestaurantService.Domain.Entities;
using RestaurantService.Infrastructure.Data;
using RestaurantDomain = RestaurantService.Domain.Entities.Restaurant;

namespace Restaurant.Infrastructure.Repositories
{
    public class RestaurantManagementRepository : IRestaurantManagementRepository
    {
        private readonly RestaurantDbContext _context;

        public RestaurantManagementRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<RestaurantDomain?> GetForManagementAsync(Guid restaurantId, CancellationToken ct = default)
        {
            var restaurant = await _context.Restaurants
                .Include(r => r.Settings)
                .Include(r => r.WorkingHours)
                .Include(r => r.Marketing)
                .FirstOrDefaultAsync(r => r.Id == restaurantId, ct);

            return restaurant;
        }

        public async Task UpdateRestaurantAsync(RestaurantDomain restaurant, CancellationToken ct = default)
        {
            restaurant.UpdatedAt = DateTime.UtcNow;
            _context.Restaurants.Update(restaurant);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateSettingsAsync(RestaurantSettings settings, CancellationToken ct = default)
        {
            _context.RestaurantSettings.Update(settings);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<RestaurantMarketing?> GetMarketingAsync(Guid restaurantId, CancellationToken ct = default)
        {
            return await _context.RestaurantMarketings
                .Include(m => m.Restaurant)
                .FirstOrDefaultAsync(m => m.RestaurantId == restaurantId, ct);
        }

        public async Task UpsertMarketingAsync(RestaurantMarketing marketing, CancellationToken ct = default)
        {
            var existing = await _context.RestaurantMarketings
                .FirstOrDefaultAsync(m => m.RestaurantId == marketing.RestaurantId, ct);

            if (existing == null)
                await _context.RestaurantMarketings.AddAsync(marketing, ct);
            else
            {
                existing.AllowsPromotions = marketing.AllowsPromotions;
                existing.IsFeatured = marketing.IsFeatured;
                existing.SearchPriority = marketing.SearchPriority;
                existing.AccountManagerName = marketing.AccountManagerName;
                existing.AccountManagerEmail = marketing.AccountManagerEmail;
            }

            await _context.SaveChangesAsync(ct);
        }

        public async Task<List<WorkingHours>> GetWorkingHoursAsync(Guid restaurantId, CancellationToken ct = default)
        {
            return await _context.WorkingHours
                .Where(wh => wh.RestaurantId == restaurantId)
                .OrderBy(wh => wh.DayOfWeek)
                .ToListAsync(ct);
        }

        public async Task UpsertWorkingHoursAsync(Guid restaurantId, List<WorkingHours> hours, CancellationToken ct = default)
        {
            var existing = await _context.WorkingHours
                .Where(wh => wh.RestaurantId == restaurantId)
                .ToListAsync(ct);

            _context.WorkingHours.RemoveRange(existing);

            foreach (var h in hours)
                h.RestaurantId = restaurantId;

            await _context.WorkingHours.AddRangeAsync(hours, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<List<RestaurantMarketing>> GetAllFeaturedOrderByPriorityAsync(CancellationToken ct)
        {
            return await _context.RestaurantMarketings
                    .Include(m => m.Restaurant)
                    .Where(m => m.IsFeatured)
                    .OrderByDescending(m => m.SearchPriority)
                    .ToListAsync(ct);
        }
    }
}
