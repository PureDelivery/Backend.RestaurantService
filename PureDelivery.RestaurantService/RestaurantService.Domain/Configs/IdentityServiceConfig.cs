namespace RestaurantService.Domain.Configs
{
    public class IdentityServiceConfig
    {
        public string BaseUrl { get; set; } = string.Empty;

        public string GetCreateCredentialEndpoint() =>
            "/api/internal/v1/identity/UserCredential";
    }
}
