using RestaurantService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Mappers.impl
{
    public class EnumConverter : IEnumConverter
    {
        public List<string> ConvertCuisineTypes(CuisineType cuisineTypes)
        {
            var result = new List<string>();

            var cuisineFlags = new[]
            {
                (CuisineType.Italian, "Italian"),
                (CuisineType.Chinese, "Chinese"),
                (CuisineType.Japanese, "Japanese"),
                (CuisineType.Mexican, "Mexican"),
                (CuisineType.Indian, "Indian"),
                (CuisineType.Thai, "Thai"),
                (CuisineType.French, "French"),
                (CuisineType.American, "American")
            };

            foreach (var (flag, name) in cuisineFlags)
            {
                if (cuisineTypes.HasFlag(flag))
                    result.Add(name);
            }

            return result;
        }

        public List<string> ConvertTags(RestaurantTag tags)
        {
            var result = new List<string>();

            var tagFlags = new[]
            {
                (RestaurantTag.FastDelivery, "FastDelivery"),
                (RestaurantTag.HealthyFood, "HealthyFood"),
                (RestaurantTag.LuxuryDining, "LuxuryDining"),
                (RestaurantTag.FamilyFriendly, "FamilyFriendly"),
                (RestaurantTag.Romantic, "Romantic"),
                (RestaurantTag.CasualDining, "CasualDining"),
                (RestaurantTag.BusinessLunch, "BusinessLunch"),
                (RestaurantTag.LateNight, "LateNight"),
                (RestaurantTag.Breakfast, "Breakfast"),
                (RestaurantTag.Buffet, "Buffet"),
                (RestaurantTag.Takeaway, "Takeaway"),
                (RestaurantTag.Catering, "Catering"),
                (RestaurantTag.Organic, "Organic"),
                (RestaurantTag.LocalIngredients, "LocalIngredients"),
                (RestaurantTag.ChefSpecial, "ChefSpecial"),
                (RestaurantTag.Budget, "Budget"),
                (RestaurantTag.Premium, "Premium"),
                (RestaurantTag.NewOpening, "NewOpening"),
                (RestaurantTag.PopularChoice, "PopularChoice")
            };

            foreach (var (flag, name) in tagFlags)
            {
                if (tags.HasFlag(flag))
                    result.Add(name);
            }

            return result;
        }
    }
}
