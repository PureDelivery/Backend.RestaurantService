using PureDelivery.Shared.Contracts.Domain.Enums;

namespace Restaurant.Application.DTOs.MenuManagement
{
    public class UpdateMenuItemOptionRequest
    {
        public string? Name { get; set; }
        public OptionType? Type { get; set; }
        public bool? IsRequired { get; set; }
        public int? SortOrder { get; set; }
    }
}
