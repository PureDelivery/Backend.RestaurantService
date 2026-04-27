// DeliveryZonesController.cs
// Простой контроллер для управления зонами доставки

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantService.Infrastructure.Data;
using RestaurantService.Domain.Entities;

namespace RestaurantService.API.Controllers
{
    [ApiController]
    [Route("api/v1/restaurant/[controller]")]
    public class DeliveryZonesController : ControllerBase
    {
        private readonly RestaurantDbContext _context;

        public DeliveryZonesController(RestaurantDbContext context)
        {
            _context = context;
        }

        // GET: api/deliveryzones/{zoneId}/points
        // Получить все точки для зоны
        [HttpGet("{zoneId}/points")]
        public async Task<ActionResult<DeliveryZoneWithPointsDto>> GetZoneWithPoints(Guid zoneId)
        {
            try
            {
                var zone = await _context.DeliveryZones
                    .Include(z => z.Points.OrderBy(p => p.Order))
                    .Include(z => z.Restaurant)
                    .FirstOrDefaultAsync(z => z.Id == zoneId);

                if (zone == null)
                {
                    return NotFound($"Zone with ID {zoneId} not found");
                }

                var result = new DeliveryZoneWithPointsDto
                {
                    Id = zone.Id,
                    Name = zone.Name,
                    RestaurantName = zone.Restaurant.Name,
                    DeliveryFee = zone.DeliveryFee,
                    MinOrderAmount = zone.MinOrderAmount,
                    EstimatedDeliveryMinutes = zone.EstimatedDeliveryMinutes,
                    IsActive = zone.IsActive,
                    Points = zone.Points.Select(p => new ZonePointDto
                    {
                        Id = p.Id,
                        Order = p.Order,
                        Latitude = p.Latitude,
                        Longitude = p.Longitude
                    }).ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving zone: {ex.Message}");
            }
        }

        // GET: api/deliveryzones
        // Получить все зоны с их точками
        [HttpGet]
        public async Task<ActionResult<List<DeliveryZoneWithPointsDto>>> GetAllZones()
        {
            try
            {
                var zones = await _context.DeliveryZones
                    .Include(z => z.Points.OrderBy(p => p.Order))
                    .Include(z => z.Restaurant)
                    .Where(z => z.IsActive)
                    .ToListAsync();

                var result = zones.Select(zone => new DeliveryZoneWithPointsDto
                {
                    Id = zone.Id,
                    Name = zone.Name,
                    RestaurantName = zone.Restaurant.Name,
                    DeliveryFee = zone.DeliveryFee,
                    MinOrderAmount = zone.MinOrderAmount,
                    EstimatedDeliveryMinutes = zone.EstimatedDeliveryMinutes,
                    IsActive = zone.IsActive,
                    Priority = zone.Priority,
                    Points = zone.Points.Select(p => new ZonePointDto
                    {
                        Id = p.Id,
                        Order = p.Order,
                        Latitude = p.Latitude,
                        Longitude = p.Longitude
                    }).ToList()
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving zones: {ex.Message}");
            }
        }

        // POST: api/deliveryzones
        // Создать новую зону с точками
        [HttpPost]
        public async Task<ActionResult<DeliveryZoneWithPointsDto>> CreateZone([FromBody] CreateDeliveryZoneDto createDto)
        {
            try
            {
                if (createDto.Points == null || createDto.Points.Count < 3)
                {
                    return BadRequest("Zone must have at least 3 points to form a polygon");
                }

                // Проверяем что ресторан существует
                var restaurantExists = await _context.Restaurants
                    .AnyAsync(r => r.Id == createDto.RestaurantId);

                if (!restaurantExists)
                {
                    return BadRequest($"Restaurant with ID {createDto.RestaurantId} not found");
                }

                // Создаем новую зону
                var newZone = new DeliveryZone
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = createDto.RestaurantId,
                    Name = createDto.Name,
                    DeliveryFee = createDto.DeliveryFee,
                    MinOrderAmount = createDto.MinOrderAmount,
                    EstimatedDeliveryMinutes = createDto.EstimatedDeliveryMinutes,
                    Priority = createDto.Priority,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.DeliveryZones.Add(newZone);

                // Создаем точки зоны
                var zonePoints = createDto.Points.Select((point, index) => new ZonePoint
                {
                    Id = Guid.NewGuid(),
                    ZoneId = newZone.Id,
                    Order = index + 1,
                    Latitude = point.Latitude,
                    Longitude = point.Longitude
                }).ToList();

                _context.ZonePoints.AddRange(zonePoints);

                await _context.SaveChangesAsync();

                // Получаем созданную зону с точками для возврата
                var createdZone = await _context.DeliveryZones
                    .Include(z => z.Points.OrderBy(p => p.Order))
                    .Include(z => z.Restaurant)
                    .FirstAsync(z => z.Id == newZone.Id);

                var result = new DeliveryZoneWithPointsDto
                {
                    Id = createdZone.Id,
                    Name = createdZone.Name,
                    RestaurantName = createdZone.Restaurant.Name,
                    DeliveryFee = createdZone.DeliveryFee,
                    MinOrderAmount = createdZone.MinOrderAmount,
                    EstimatedDeliveryMinutes = createdZone.EstimatedDeliveryMinutes,
                    IsActive = createdZone.IsActive,
                    Points = createdZone.Points.Select(p => new ZonePointDto
                    {
                        Id = p.Id,
                        Order = p.Order,
                        Latitude = p.Latitude,
                        Longitude = p.Longitude
                    }).ToList()
                };

                return CreatedAtAction(nameof(GetZoneWithPoints), new { zoneId = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating zone: {ex.Message}");
            }
        }

        // GET: api/deliveryzones/restaurant/{restaurantId}
        // Получить все зоны для конкретного ресторана (для менеджера)
        [HttpGet("restaurant/{restaurantId:guid}")]
        public async Task<ActionResult<List<DeliveryZoneWithPointsDto>>> GetZonesByRestaurant(Guid restaurantId)
        {
            try
            {
                var zones = await _context.DeliveryZones
                    .Include(z => z.Points.OrderBy(p => p.Order))
                    .Include(z => z.Restaurant)
                    .Where(z => z.RestaurantId == restaurantId)
                    .ToListAsync();

                var result = zones.Select(zone => new DeliveryZoneWithPointsDto
                {
                    Id = zone.Id,
                    Name = zone.Name,
                    RestaurantName = zone.Restaurant.Name,
                    DeliveryFee = zone.DeliveryFee,
                    MinOrderAmount = zone.MinOrderAmount,
                    EstimatedDeliveryMinutes = zone.EstimatedDeliveryMinutes,
                    IsActive = zone.IsActive,
                    Priority = zone.Priority,
                    Points = zone.Points.Select(p => new ZonePointDto
                    {
                        Id = p.Id,
                        Order = p.Order,
                        Latitude = p.Latitude,
                        Longitude = p.Longitude
                    }).ToList()
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving zones for restaurant: {ex.Message}");
            }
        }

        // GET: api/deliveryzones/restaurants
        // Получить список ресторанов для выбора
        [HttpGet("restaurants")]
        public async Task<ActionResult<List<RestaurantDtoShort>>> GetRestaurants()
        {
            try
            {
                var restaurants = await _context.Restaurants
                    .Where(r => r.IsActive)
                    .Select(r => new RestaurantDtoShort
                    {
                        Id = r.Id,
                        Name = r.Name
                    })
                    .ToListAsync();

                return Ok(restaurants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving restaurants: {ex.Message}");
            }
        }

        // DELETE: api/deliveryzones/{zoneId}
        // Удалить зону
        [HttpDelete("{zoneId}")]
        public async Task<ActionResult> DeleteZone(Guid zoneId)
        {
            try
            {
                var zone = await _context.DeliveryZones
                    .Include(z => z.Points)
                    .FirstOrDefaultAsync(z => z.Id == zoneId);

                if (zone == null)
                {
                    return NotFound($"Zone with ID {zoneId} not found");
                }

                _context.ZonePoints.RemoveRange(zone.Points);
                _context.DeliveryZones.Remove(zone);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting zone: {ex.Message}");
            }
        }
    }

    // DTOs для API
    public class DeliveryZoneWithPointsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string RestaurantName { get; set; } = string.Empty;
        public decimal DeliveryFee { get; set; }
        public decimal MinOrderAmount { get; set; }
        public int EstimatedDeliveryMinutes { get; set; }
        public bool IsActive { get; set; }
        public int Priority { get; set; }
        public List<ZonePointDto> Points { get; set; } = new();
    }

    public class ZonePointDto
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class CreateDeliveryZoneDto
    {
        public Guid RestaurantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal DeliveryFee { get; set; }
        public decimal MinOrderAmount { get; set; }
        public int EstimatedDeliveryMinutes { get; set; }
        public int Priority { get; set; } = 1;
        public List<CreateZonePointDto> Points { get; set; } = new();
    }

    public class CreateZonePointDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class RestaurantDtoShort
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}