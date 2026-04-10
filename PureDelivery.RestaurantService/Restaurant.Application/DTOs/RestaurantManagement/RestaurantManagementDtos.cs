namespace Restaurant.Application.DTOs.RestaurantManagement
{
    public class RestaurantManagementDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<string> CuisineTypes { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public RestaurantSettingsManagementDto Settings { get; set; } = new();
        public List<WorkingHoursManagementDto> WorkingHours { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class RestaurantSettingsManagementDto
    {
        public decimal MinOrderAmount { get; set; }
        public bool AcceptPreOrders { get; set; }
        public int MaxPreOrderDays { get; set; }
        public int AveragePreparationMinutes { get; set; }
        public int MaxPreparationMinutes { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
    }

    public class WorkingHoursManagementDto
    {
        public Guid Id { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public string OpenTime { get; set; } = string.Empty;
        public string CloseTime { get; set; } = string.Empty;
        public bool IsClosed { get; set; }
    }
}
