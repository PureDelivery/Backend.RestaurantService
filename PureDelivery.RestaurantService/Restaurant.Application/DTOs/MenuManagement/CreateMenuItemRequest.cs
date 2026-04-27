using PureDelivery.Shared.Contracts.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.Application.DTOs.MenuManagement
{
    public class CreateMenuItemRequest
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required, Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public MenuCategory Category { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int PreparationTimeMinutes { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Required]
        public CreateNutritionInfoRequest Nutrition { get; set; } = null!;
    }

    public class CreateNutritionInfoRequest
    {
        public int CaloriesPer100g { get; set; }
        public decimal ProteinPer100g { get; set; }
        public decimal FatPer100g { get; set; }
        public decimal CarbsPer100g { get; set; }
        public int WeightGrams { get; set; }
        public Allergen Allergens { get; set; } = Allergen.None;
        public DietaryTag DietaryTags { get; set; } = DietaryTag.None;
    }
}
