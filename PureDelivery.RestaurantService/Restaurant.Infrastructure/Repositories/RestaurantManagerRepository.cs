using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Repositories;
using RestaurantService.Domain.Entities;
using RestaurantService.Infrastructure.Data;

namespace Restaurant.Infrastructure.Repositories
{
    public class RestaurantManagerRepository : IRestaurantManagerRepository
    {
        private readonly RestaurantDbContext _context;

        public RestaurantManagerRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<RestaurantManager?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            await _context.RestaurantManagers
                .FirstOrDefaultAsync(m => m.Id == id && m.IsActive, ct);

        public async Task<RestaurantManager?> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
            await _context.RestaurantManagers
                .FirstOrDefaultAsync(m => m.UserId == userId && m.IsActive, ct);

        public async Task<List<RestaurantManager>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken ct = default) =>
            await _context.RestaurantManagers
                .Where(m => m.RestaurantId == restaurantId && m.IsActive)
                .ToListAsync(ct);

        public async Task<RestaurantManager> AddAsync(RestaurantManager manager, CancellationToken ct = default)
        {
            await _context.RestaurantManagers.AddAsync(manager, ct);
            await _context.SaveChangesAsync(ct);
            return manager;
        }

        public async Task<bool> DeactivateAsync(Guid id, CancellationToken ct = default)
        {
            var manager = await _context.RestaurantManagers.FindAsync([id], ct);
            if (manager == null) return false;

            manager.IsActive = false;
            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}
