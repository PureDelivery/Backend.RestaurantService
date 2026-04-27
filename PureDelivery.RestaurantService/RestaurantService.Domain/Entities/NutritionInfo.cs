using PureDelivery.Shared.Contracts.Domain.Enums;

namespace RestaurantService.Domain.Entities
{
    public class NutritionInfo
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid MenuItemId { get; set; }

        public int CaloriesPer100g { get; set; }
        public decimal ProteinPer100g { get; set; }
        public decimal FatPer100g { get; set; }
        public decimal CarbsPer100g { get; set; }
        public int WeightGrams { get; set; }

        // JSON списки аллергенов и диет
        public Allergen Allergens { get; set; } = Allergen.None;
        public DietaryTag DietaryTags { get; set; } = DietaryTag.None;

        public virtual MenuItem MenuItem { get; set; } = null!;
    }
}
