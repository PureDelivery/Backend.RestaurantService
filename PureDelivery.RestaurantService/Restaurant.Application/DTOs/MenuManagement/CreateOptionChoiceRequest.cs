using System.ComponentModel.DataAnnotations;

namespace Restaurant.Application.DTOs.MenuManagement
{
    public class CreateOptionChoiceRequest
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public decimal PriceModifier { get; set; } = 0;

        public bool IsDefault { get; set; } = false;

        public bool IsAvailable { get; set; } = true;

        public int SortOrder { get; set; } = 0;
    }
}
