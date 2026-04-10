using RestaurantService.Domain.Enums;

namespace Restaurant.Application.Mappers
{
    public interface IEnumConverter
    {
        List<string> ConvertCuisineTypes(CuisineType cuisineTypes);
        List<string> ConvertTags(RestaurantTag tags);
    }
}
