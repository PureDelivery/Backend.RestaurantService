using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantService.Domain.Entities
{
    public class RestaurantReview
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RestaurantId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid OrderId { get; set; }

        [Range(1, 5, ErrorMessage = "Рейтинг должен быть от 1 до 5 звезд")]
        public int Rating { get; set; } // 1-5 звезд

        [MaxLength(1000, ErrorMessage = "Комментарий не может превышать 1000 символов")]
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public virtual Restaurant Restaurant { get; set; } = null!;
    }
}
