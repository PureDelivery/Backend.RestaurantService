public class LocationServiceConfig
{
    public string BaseUrl { get; set; } = string.Empty;
    public Dictionary<string, string> Endpoints { get; set; } = new Dictionary<string, string>();

    // Константы для эндпоинтов
    public static class EndpointKeys
    {
        public const string FILTER_RESTAURANTS_BY_LOCATION = "FilterRestaurantsByLocation";
        public const string CHECK_POINT_IN_ZONE = "CheckPointInZone";
    }

    // Вспомогательные методы для получения URL эндпоинтов
    public string GetFilterRestaurantsEndpoint()
    {
        return Endpoints.GetValueOrDefault(EndpointKeys.FILTER_RESTAURANTS_BY_LOCATION, "/api/location/filter-restaurants");
    }

    public string GetCheckPointInZoneEndpoint()
    {
        return Endpoints.GetValueOrDefault(EndpointKeys.CHECK_POINT_IN_ZONE, "/api/location/check-point-in-zone");
    }

    public string GetFullUrl(string endpoint)
    {
        return $"{BaseUrl.TrimEnd('/')}{endpoint}";
    }
}