using System.ComponentModel.DataAnnotations;

namespace Restaurant.Application.DTOs.AdminMarketing
{
    public class UpdateRestaurantMarketingRequest
    {
        public bool? AllowsPromotions { get; set; }

        public bool? IsFeatured { get; set; }

        /// <summary>
        /// Приоритет в поиске: 1 (обычный) → 10 (максимальный буст).
        /// </summary>
        [Range(1, 10)]
        public int? SearchPriority { get; set; }

        [MaxLength(200)]
        public string? AccountManagerName { get; set; }

        [EmailAddress, MaxLength(256)]
        public string? AccountManagerEmail { get; set; }
    }
}
