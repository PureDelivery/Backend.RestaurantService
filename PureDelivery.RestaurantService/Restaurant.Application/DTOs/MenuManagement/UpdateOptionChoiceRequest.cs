namespace Restaurant.Application.DTOs.MenuManagement
{
    public class UpdateOptionChoiceRequest
    {
        public string? Name { get; set; }
        public decimal? PriceModifier { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsAvailable { get; set; }
        public int? SortOrder { get; set; }
    }
}
