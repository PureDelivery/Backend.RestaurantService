namespace Restaurant.Application.DTOs.AdminMarketing
{
    public class AdminMarketingDto
    {
        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public bool AllowsPromotions { get; set; }
        public bool IsFeatured { get; set; }
        public int SearchPriority { get; set; }
        public string AccountManagerName { get; set; } = string.Empty;
        public string AccountManagerEmail { get; set; } = string.Empty;
    }
}
