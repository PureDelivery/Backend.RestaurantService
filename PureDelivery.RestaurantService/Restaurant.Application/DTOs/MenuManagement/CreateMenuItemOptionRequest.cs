using RestaurantService.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.Application.DTOs.MenuManagement
{
    public class CreateMenuItemOptionRequest
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public OptionType Type { get; set; } = OptionType.Single;

        public bool IsRequired { get; set; } = false;

        public int SortOrder { get; set; } = 0;
    }
}
