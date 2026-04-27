using PureDelivery.Shared.Contracts.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.Application.DTOs.RestaurantManagement
{
    public class UpdateRestaurantInfoRequest
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public CuisineType? CuisineTypes { get; set; }

        public RestaurantTag? Tags { get; set; }
    }
}
