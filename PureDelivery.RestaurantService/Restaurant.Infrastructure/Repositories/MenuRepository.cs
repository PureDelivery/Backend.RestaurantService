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

    // ── Customer-facing (only available items) ──────────────────────────────

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

        if (menuItem == null) return null;

        await _context.Entry(menuItem).Reference(mi => mi.Nutrition).LoadAsync();
        await _context.Entry(menuItem).Reference(mi => mi.Popularity).LoadAsync();
        await _context.Entry(menuItem).Collection(mi => mi.Options).LoadAsync();

        foreach (var option in menuItem.Options)
            await _context.Entry(option).Collection(o => o.Choices).LoadAsync();

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

    public Task<List<MenuItem>> GetMenuItemsByIdsAsync(List<Guid> itemIds)
    {
        return _context.MenuItems
            .Include(mi => mi.Options)
                .ThenInclude(o => o.Choices)
            .Where(mi => itemIds.Contains(mi.Id))
            .ToListAsync();
    }

    // ── Management (all items including unavailable) ─────────────────────────

    public async Task<List<MenuItem>> GetAllMenuItemsForManagementAsync(Guid restaurantId, CancellationToken ct = default)
    {
        return await _context.MenuItems
            .Include(mi => mi.Options)
                .ThenInclude(o => o.Choices)
            .Where(mi => mi.RestaurantId == restaurantId)
            .OrderBy(mi => mi.Category)
            .ThenBy(mi => mi.Name)
            .ToListAsync(ct);
    }

    public async Task<MenuItem?> GetMenuItemRawAsync(Guid restaurantId, Guid itemId, CancellationToken ct = default)
    {
        return await _context.MenuItems
            .Include(mi => mi.Options)
                .ThenInclude(o => o.Choices)
            .FirstOrDefaultAsync(mi => mi.Id == itemId && mi.RestaurantId == restaurantId, ct);
    }

    public async Task<MenuItemOption?> GetOptionAsync(Guid itemId, Guid optionId, CancellationToken ct = default)
    {
        return await _context.MenuItemOptions
            .Include(o => o.Choices)
            .FirstOrDefaultAsync(o => o.Id == optionId && o.MenuItemId == itemId, ct);
    }

    public async Task<OptionChoice?> GetChoiceAsync(Guid optionId, Guid choiceId, CancellationToken ct = default)
    {
        return await _context.OptionChoices
            .FirstOrDefaultAsync(c => c.Id == choiceId && c.OptionId == optionId, ct);
    }

    // ── Write: MenuItem ──────────────────────────────────────────────────────

    public async Task<MenuItem> AddMenuItemAsync(MenuItem item, CancellationToken ct = default)
    {
        await _context.MenuItems.AddAsync(item, ct);
        await _context.SaveChangesAsync(ct);
        return item;
    }

    public async Task UpdateMenuItemAsync(MenuItem item, CancellationToken ct = default)
    {
        item.UpdatedAt = DateTime.UtcNow;
        _context.MenuItems.Update(item);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteMenuItemAsync(MenuItem item, CancellationToken ct = default)
    {
        _context.MenuItems.Remove(item);
        await _context.SaveChangesAsync(ct);
    }

    // ── Write: MenuItemOption ────────────────────────────────────────────────

    public async Task<MenuItemOption> AddOptionAsync(MenuItemOption option, CancellationToken ct = default)
    {
        await _context.MenuItemOptions.AddAsync(option, ct);
        await _context.SaveChangesAsync(ct);
        return option;
    }

    public async Task UpdateOptionAsync(MenuItemOption option, CancellationToken ct = default)
    {
        _context.MenuItemOptions.Update(option);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteOptionAsync(MenuItemOption option, CancellationToken ct = default)
    {
        _context.MenuItemOptions.Remove(option);
        await _context.SaveChangesAsync(ct);
    }

    // ── Write: OptionChoice ──────────────────────────────────────────────────

    public async Task<OptionChoice> AddChoiceAsync(OptionChoice choice, CancellationToken ct = default)
    {
        await _context.OptionChoices.AddAsync(choice, ct);
        await _context.SaveChangesAsync(ct);
        return choice;
    }

    public async Task UpdateChoiceAsync(OptionChoice choice, CancellationToken ct = default)
    {
        _context.OptionChoices.Update(choice);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteChoiceAsync(OptionChoice choice, CancellationToken ct = default)
    {
        _context.OptionChoices.Remove(choice);
        await _context.SaveChangesAsync(ct);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static bool HasMenuCategory(MenuCategory menuItemCategory, string categoryName)
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
}
