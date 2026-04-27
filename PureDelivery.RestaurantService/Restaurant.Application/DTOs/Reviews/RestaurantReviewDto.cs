namespace Restaurant.Application.DTOs.Reviews;

public class RestaurantReviewDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid OrderId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PagedReviewsDto
{
    public List<RestaurantReviewDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public double AverageRating { get; set; }
}
