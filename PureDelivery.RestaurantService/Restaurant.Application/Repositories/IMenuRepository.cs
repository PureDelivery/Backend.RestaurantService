using RestaurantService.Domain.Entities;

namespace Restaurant.Application.Repositories
{
    public interface IMenuRepository
    {
        // --- Read (customer-facing, only IsAvailable items) ---
        Task<List<MenuItem>> GetMenuItemsByRestaurantIdAsync(Guid restaurantId);
        Task<MenuItem?> GetMenuItemByIdAsync(Guid restaurantId, Guid menuItemId);
        Task<List<MenuItem>> SearchMenuItemsInRestaurantAsync(Guid restaurantId, string query);
        Task<List<MenuItem>> FilterMenuItemsByCategoryAsync(Guid restaurantId, List<string> categories);
        Task<List<MenuItem>> GetMenuItemsByIdsAsync(List<Guid> itemIds);

        // --- Read (management, all items including unavailable) ---
        Task<List<MenuItem>> GetAllMenuItemsForManagementAsync(Guid restaurantId, CancellationToken ct = default);
        Task<MenuItem?> GetMenuItemRawAsync(Guid restaurantId, Guid itemId, CancellationToken ct = default);
        Task<MenuItemOption?> GetOptionAsync(Guid itemId, Guid optionId, CancellationToken ct = default);
        Task<OptionChoice?> GetChoiceAsync(Guid optionId, Guid choiceId, CancellationToken ct = default);

        // --- Write: MenuItem ---
        Task<MenuItem> AddMenuItemAsync(MenuItem item, CancellationToken ct = default);
        Task UpdateMenuItemAsync(MenuItem item, CancellationToken ct = default);
        Task DeleteMenuItemAsync(MenuItem item, CancellationToken ct = default);

        // --- Write: MenuItemOption ---
        Task<MenuItemOption> AddOptionAsync(MenuItemOption option, CancellationToken ct = default);
        Task UpdateOptionAsync(MenuItemOption option, CancellationToken ct = default);
        Task DeleteOptionAsync(MenuItemOption option, CancellationToken ct = default);

        // --- Write: OptionChoice ---
        Task<OptionChoice> AddChoiceAsync(OptionChoice choice, CancellationToken ct = default);
        Task UpdateChoiceAsync(OptionChoice choice, CancellationToken ct = default);
        Task DeleteChoiceAsync(OptionChoice choice, CancellationToken ct = default);
    }
}
