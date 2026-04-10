namespace RestaurantService.Domain.Entities
{
    public class RestaurantManager
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// UserId из IdentityService — используется для аутентификации.
        /// </summary>
        public Guid UserId { get; set; }

        public Guid RestaurantId { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual Restaurant? Restaurant { get; set; }
    }
}
