using PureDelivery.Shared.Contracts.Domain.Enums;
using PureDelivery.Shared.Contracts.DTOs.Restaurants.Responses;
using RestaurantService.Domain.Entities;

namespace Restaurant.Application.Mappers.impl
{
    public class MenuMapper : IMenuMapper
    {
        public RestaurantMenuDto MapToMenuDto(Guid restaurantId, string restaurantName, List<MenuItem> menuItems)
        {
            var groupedByCategory = menuItems
                .GroupBy(mi => mi.Category)
                .OrderBy(g => g.Key)
                .Select(g => new MenuCategoryDto
                {
                    CategoryId = Guid.NewGuid(),
                    CategoryName = GetCategoryDisplayName(g.Key),
                    Items = g.Select(MapToDto).ToList()
                })
                .ToList();

            return new RestaurantMenuDto
            {
                RestaurantId = restaurantId,
                RestaurantName = restaurantName,
                Categories = groupedByCategory
            };
        }

        public MenuItemDetailDto MapToDetailDto(MenuItem menuItem)
        {
            return new MenuItemDetailDto
            {
                Id = menuItem.Id,
                RestaurantId = menuItem.RestaurantId,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                ImageUrl = menuItem.ImageUrl,
                MenuCategory = GetCategoryDisplayName(menuItem.Category),
                IsAvailable = menuItem.IsAvailable,
                PreparationTimeMinutes = menuItem.PreparationTimeMinutes,
                IsPopular = menuItem.Popularity?.IsPopular ?? false,
                IsRecommended = menuItem.Popularity?.IsRecommended ?? false,
                IsNew = menuItem.Popularity?.IsNew ?? false,
                Nutrition = MapNutritionDto(menuItem.Nutrition),
                Options = menuItem.Options.Select(MapOptionDto).ToList()
            };
        }

        public List<MenuItemDto> MapToDtos(IEnumerable<MenuItem> menuItems)
        {
            return menuItems.Select(MapToDto).ToList();
        }

        private MenuItemDto MapToDto(MenuItem menuItem)
        {
            return new MenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                ImageUrl = menuItem.ImageUrl,
                IsAvailable = menuItem.IsAvailable,
                PreparationTimeMinutes = menuItem.PreparationTimeMinutes,
                IsPopular = menuItem.Popularity?.IsPopular ?? false,
                IsRecommended = menuItem.Popularity?.IsRecommended ?? false,
                IsNew = menuItem.Popularity?.IsNew ?? false
            };
        }

        private NutritionInfoDto? MapNutritionDto(NutritionInfo? nutrition)
        {
            if (nutrition == null)
                return null;

            return new NutritionInfoDto
            {
                CaloriesPer100g = nutrition.CaloriesPer100g,
                ProteinPer100g = nutrition.ProteinPer100g,
                FatPer100g = nutrition.FatPer100g,
                CarbsPer100g = nutrition.CarbsPer100g,
                WeightGrams = nutrition.WeightGrams,
                Allergens = ConvertAllergens(nutrition.Allergens),
                DietaryTags = ConvertDietaryTags(nutrition.DietaryTags)
            };
        }

        private MenuItemOptionDto MapOptionDto(MenuItemOption option)
        {
            return new MenuItemOptionDto
            {
                Id = option.Id,
                Name = option.Name,
                OptionType = GetOptionTypeDisplayName(option.Type),
                IsRequired = option.IsRequired,
                Choices = option.Choices.Select(MapChoiceDto).ToList()
            };
        }

        private OptionChoiceDto MapChoiceDto(OptionChoice choice)
        {
            return new OptionChoiceDto
            {
                Id = choice.Id,
                Name = choice.Name,
                PriceModifier = choice.PriceModifier,
                IsDefault = choice.IsDefault,
                IsAvailable = choice.IsAvailable
            };
        }

        private string GetCategoryDisplayName(MenuCategory category)
        {
            return category switch
            {
                MenuCategory.Appetizers => "Appetizers",
                MenuCategory.Soups => "Soups",
                MenuCategory.MainCourse => "MainCourse",
                MenuCategory.Pizza => "Pizza",
                MenuCategory.Pasta => "Pasta",
                MenuCategory.Salads => "Salads",
                MenuCategory.Desserts => "Desserts",
                MenuCategory.Beverages => "Beverages",
                MenuCategory.AlcoholicDrinks => "AlcoholicDrinks",
                MenuCategory.Sides => "Sides",
                MenuCategory.Seafood => "Seafood",
                MenuCategory.Meat => "Meat",
                MenuCategory.Vegetarian => "Vegetarian",
                MenuCategory.Breakfast => "Breakfast",
                MenuCategory.Lunch => "Lunch",
                MenuCategory.Dinner => "Dinner",
                _ => "Other"
            };
        }

        private string GetOptionTypeDisplayName(OptionType type)
        {
            return type switch
            {
                OptionType.Single => "Single",
                OptionType.Multiple => "Multiple",
                _ => "Single"
            };
        }

        private List<string> ConvertAllergens(Allergen allergens)
        {
            var result = new List<string>();

            var allergenFlags = new[]
            {
            (Allergen.Gluten, "Gluten"),
            (Allergen.Dairy, "Dairy"),
            (Allergen.Eggs, "Eggs"),
            (Allergen.Fish, "Fish"),
            (Allergen.Shellfish, "Shellfish"),
            (Allergen.TreeNuts, "TreeNuts"),
            (Allergen.Peanuts, "Peanuts"),
            (Allergen.Soy, "Soy"),
            (Allergen.Sesame, "Sesame"),
            (Allergen.Sulfites, "Sulfites"),
            (Allergen.Celery, "Celery"),
            (Allergen.Mustard, "Mustard"),
            (Allergen.Lupin, "Lupin")
        };

            foreach (var (flag, name) in allergenFlags)
            {
                if (allergens.HasFlag(flag))
                    result.Add(name);
            }

            return result;
        }

        private List<string> ConvertDietaryTags(DietaryTag dietaryTags)
        {
            var result = new List<string>();

            var dietaryFlags = new[]
            {
            (DietaryTag.Vegetarian, "Vegetarian"),
            (DietaryTag.Vegan, "Vegan"),
            (DietaryTag.GlutenFree, "GlutenFree"),
            (DietaryTag.DairyFree, "DairyFree"),
            (DietaryTag.Organic, "Organic"),
            (DietaryTag.LowCarb, "LowCarb"),
            (DietaryTag.LowFat, "LowFat"),
            (DietaryTag.HighProtein, "HighProtein"),
            (DietaryTag.Keto, "Keto"),
            (DietaryTag.Paleo, "Paleo"),
            (DietaryTag.Halal, "Halal"),
            (DietaryTag.Kosher, "Kosher"),
            (DietaryTag.Spicy, "Spicy"),
            (DietaryTag.LowSodium, "LowSodium"),
            (DietaryTag.SugarFree, "SugarFree")
        };

            foreach (var (flag, name) in dietaryFlags)
            {
                if (dietaryTags.HasFlag(flag))
                    result.Add(name);
            }

            return result;
        }
    }
}
