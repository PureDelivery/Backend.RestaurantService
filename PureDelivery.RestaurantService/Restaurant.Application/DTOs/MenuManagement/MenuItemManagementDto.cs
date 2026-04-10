namespace Restaurant.Application.DTOs.MenuManagement
{
    public class MenuItemManagementDto
    {
        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public string? UnavailabilityReason { get; set; }
        public int PreparationTimeMinutes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<OptionManagementDto> Options { get; set; } = new();
    }

    public class OptionManagementDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public int SortOrder { get; set; }
        public List<ChoiceManagementDto> Choices { get; set; } = new();
    }

    public class ChoiceManagementDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal PriceModifier { get; set; }
        public bool IsDefault { get; set; }
        public bool IsAvailable { get; set; }
        public int SortOrder { get; set; }
    }
}
