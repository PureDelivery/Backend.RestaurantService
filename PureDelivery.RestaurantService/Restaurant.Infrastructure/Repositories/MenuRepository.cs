using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Repositories;
using RestaurantService.Domain.Entities;
using RestaurantService.Domain.Enums;
using RestaurantService.Infrastructure.Data;

namespace Restaurant.Infrastructure.Repositories;

public class MenuRepository : IMenuRepository
{
    private readonly RestaurantDbContext _context;

    public MenuRepository(RestaurantDbContext context)
    {
        _context = context;
    }

    public async Task<List<MenuItem>> GetMenuItemsByRestaurantIdAsync(Guid restaurantId)
    {
        return await _context.MenuItems
            .Include(mi => mi.Nutrition)
            .Include(mi => mi.Popularity)
            .Include(mi => mi.Options)
                .ThenInclude(o => o.Choices)
            .Where(mi => mi.RestaurantId == restaurantId && mi.IsAvailable)
            .OrderBy(mi => mi.Category)
            .ThenBy(mi => mi.Name)
            .ToListAsync();
    }

    public async Task<MenuItem?> GetMenuItemByIdAsync(Guid restaurantId, Guid menuItemId)
    {
        var menuItem = await _context.MenuItems
            .FirstOrDefaultAsync(mi => mi.Id == menuItemId &&
                                      mi.RestaurantId == restaurantId &&
                                      mi.IsAvailable);

        if (menuItem == null)
            return null;

        // Lazy loading для детального просмотра
        await _context.Entry(menuItem)
            .Reference(mi => mi.Nutrition)
            .LoadAsync();

        await _context.Entry(menuItem)
            .Reference(mi => mi.Popularity)
            .LoadAsync();

        await _context.Entry(menuItem)
            .Collection(mi => mi.Options)
            .LoadAsync();

        // Загружаем choices для каждой опции
        foreach (var option in menuItem.Options)
        {
            await _context.Entry(option)
                .Collection(o => o.Choices)
                .LoadAsync();
        }

        return menuItem;
    }

    public async Task<List<MenuItem>> SearchMenuItemsInRestaurantAsync(Guid restaurantId, string query)
    {
        return await _context.MenuItems
            .Include(mi => mi.Nutrition)
            .Include(mi => mi.Popularity)
            .Where(mi => mi.RestaurantId == restaurantId &&
                        mi.IsAvailable &&
                        (mi.Name.Contains(query) || mi.Description.Contains(query)))
            .OrderBy(mi => mi.Name)
            .ToListAsync();
    }

    public async Task<List<MenuItem>> FilterMenuItemsByCategoryAsync(Guid restaurantId, List<string> categories)
    {
        return await _context.MenuItems
            .Include(mi => mi.Nutrition)
            .Include(mi => mi.Popularity)
            .Where(mi => mi.RestaurantId == restaurantId &&
                        mi.IsAvailable &&
                        categories.Any(cat => HasMenuCategory(mi.Category, cat)))
            .OrderBy(mi => mi.Category)
            .ThenBy(mi => mi.Name)
            .ToListAsync();
    }

    private bool HasMenuCategory(MenuCategory menuItemCategory, string categoryName)
    {
        return categoryName switch
        {
            "Appetizers" => menuItemCategory == MenuCategory.Appetizers,
            "Soups" => menuItemCategory == MenuCategory.Soups,
            "MainCourse" => menuItemCategory == MenuCategory.MainCourse,
            "Pizza" => menuItemCategory == MenuCategory.Pizza,
            "Pasta" => menuItemCategory == MenuCategory.Pasta,
            "Salads" => menuItemCategory == MenuCategory.Salads,
            "Desserts" => menuItemCategory == MenuCategory.Desserts,
            "Beverages" => menuItemCategory == MenuCategory.Beverages,
            "AlcoholicDrinks" => menuItemCategory == MenuCategory.AlcoholicDrinks,
            "Sides" => menuItemCategory == MenuCategory.Sides,
            "Seafood" => menuItemCategory == MenuCategory.Seafood,
            "Meat" => menuItemCategory == MenuCategory.Meat,
            "Vegetarian" => menuItemCategory == MenuCategory.Vegetarian,
            "Breakfast" => menuItemCategory == MenuCategory.Breakfast,
            "Lunch" => menuItemCategory == MenuCategory.Lunch,
            "Dinner" => menuItemCategory == MenuCategory.Dinner,
            _ => false
        };
    }

    public Task<List<MenuItem>> GetMenuItemsByIdsAsync(List<Guid> itemIds)
    {
        var menuItems = _context
            .MenuItems
            .Include(mi => mi.Options)
                .ThenInclude(o => o.Choices)
            .Where(mi => itemIds.Contains(mi.Id))
            .ToListAsync();

        return menuItems;

    }
}