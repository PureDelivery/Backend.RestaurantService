using System.ComponentModel.DataAnnotations;

namespace Restaurant.Application.DTOs.RestaurantManagement
{
    public class UpsertWorkingHoursRequest
    {
        [Required]
        public List<WorkingHourEntryDto> Hours { get; set; } = new();
    }

    public class WorkingHourEntryDto
    {
        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan OpenTime { get; set; }

        public TimeSpan CloseTime { get; set; }

        public bool IsClosed { get; set; } = false;
    }
}
