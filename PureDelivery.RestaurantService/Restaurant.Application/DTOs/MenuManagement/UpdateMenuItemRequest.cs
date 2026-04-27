using PureDelivery.Shared.Contracts.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.Application.DTOs.MenuManagement
{
    public class UpdateMenuItemRequest
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? Price { get; set; }

        public MenuCategory? Category { get; set; }

        public string? ImageUrl { get; set; }

        [Range(0, int.MaxValue)]
        public int? PreparationTimeMinutes { get; set; }

        public bool? IsAvailable { get; set; }

        public string? UnavailabilityReason { get; set; }
    }
}
