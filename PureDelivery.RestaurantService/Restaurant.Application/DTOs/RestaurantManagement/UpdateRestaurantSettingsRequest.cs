using System.ComponentModel.DataAnnotations;

namespace Restaurant.Application.DTOs.RestaurantManagement
{
    public class UpdateRestaurantSettingsRequest
    {
        [Range(0, double.MaxValue)]
        public decimal? MinOrderAmount { get; set; }

        public bool? AcceptPreOrders { get; set; }

        [Range(1, 30)]
        public int? MaxPreOrderDays { get; set; }

        [Range(1, 180)]
        public int? AveragePreparationMinutes { get; set; }

        [Range(1, 360)]
        public int? MaxPreparationMinutes { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [EmailAddress, MaxLength(256)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Website { get; set; }
    }
}
