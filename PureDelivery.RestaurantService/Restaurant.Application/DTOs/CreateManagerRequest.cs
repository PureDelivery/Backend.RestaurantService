using System.ComponentModel.DataAnnotations;

namespace Restaurant.Application.DTOs
{
    public class CreateManagerRequest
    {
        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public Guid RestaurantId { get; set; }
    }
}
