using System.ComponentModel.DataAnnotations;

namespace Restaurant.Application.DTOs.Reviews;

public class SubmitReviewRequest
{
    [Required]
    public Guid OrderId { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    [Required, Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }

    public string? CustomerName { get; set; }
}
