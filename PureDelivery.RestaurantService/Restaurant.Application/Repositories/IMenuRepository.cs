using RestaurantService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Repositories
{
    public interface IMenuRepository
    {
        Task<List<MenuItem>> GetMenuItemsByRestaurantIdAsync(Guid restaurantId);
        Task<MenuItem?> GetMenuItemByIdAsync(Guid restaurantId, Guid menuItemId);
        Task<List<MenuItem>> SearchMenuItemsInRestaurantAsync(Guid restaurantId, string query);
        Task<List<MenuItem>> FilterMenuItemsByCategoryAsync(Guid restaurantId, List<string> categories);
        Task<List<MenuItem>> GetMenuItemsByIdsAsync(List<Guid> itemIds);
    }
}
