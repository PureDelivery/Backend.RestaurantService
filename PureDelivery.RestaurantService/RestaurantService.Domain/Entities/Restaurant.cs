using PureDelivery.Shared.Contracts.Domain.Enums;

namespace RestaurantService.Domain.Entities
{
    public class Restaurant
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public CuisineType CuisineTypes { get; set; } = CuisineType.None;
        public RestaurantTag Tags { get; set; } = RestaurantTag.None;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual Address Address { get; set; } = new();
        public virtual RestaurantSettings Settings { get; set; } = new();
        public virtual PartnershipInfo Partnership { get; set; } = new();
        public virtual RestaurantMarketing? Marketing { get; set; }
        public virtual ICollection<RestaurantReview> Reviews { get; set; } = new List<RestaurantReview>();
        public virtual ICollection<WorkingHours> WorkingHours { get; set; } = new List<WorkingHours>();
        public virtual ICollection<DeliveryZone> DeliveryZones { get; set; } = new List<DeliveryZone>();
        public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

    }
    
}