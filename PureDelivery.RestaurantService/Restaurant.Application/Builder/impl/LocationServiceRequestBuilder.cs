using PureDelivery.Common.Configuration.Services;
using PureDelivery.Common.Http.Configuration;
using PureDelivery.Common.Http.Models;
using PureDelivery.Shared.Contracts.DTOs.Location.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Builder.impl
{
    public class LocationServiceRequestBuilder : IHttpRequestBuilder
    {
        private readonly LocationServiceConfig _locationServiceConfig;
        private readonly HttpClientConfiguration _httpConfig;

        public LocationServiceRequestBuilder(ICustomConfigurationProvider configProvider)
        {
            _locationServiceConfig = configProvider.GetConfigurationAsync<LocationServiceConfig>("LocationService").GetAwaiter().GetResult();
            _httpConfig = configProvider.GetConfigurationAsync<HttpClientConfiguration>("HttpClient").GetAwaiter().GetResult();
        }

        public HttpRequestParams BuildFilterRestaurantsRequest(FilterRestaurantsByLocationRequest filterRequest)
        {
            return HttpRequestParams
                .WithBody(
                    _locationServiceConfig.BaseUrl,
                    _locationServiceConfig.GetFilterRestaurantsEndpoint(),
                    filterRequest
                )
                .WithHeaders(BuildHeaders())
                .WithCancellation(CancellationToken.None);
        }

        private Dictionary<string, string> BuildHeaders()
        {
            var headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Accept", "application/json" },
                { "User-Agent", _httpConfig.UserAgent }
            };

            foreach (var defaultHeader in _httpConfig.DefaultHeaders)
            {
                if (!headers.ContainsKey(defaultHeader.Key))
                {
                    headers[defaultHeader.Key] = defaultHeader.Value;
                }
            }

            if (!string.IsNullOrEmpty(_httpConfig.BearerToken))
            {
                headers["Authorization"] = $"Bearer {_httpConfig.BearerToken}";
            }

            if (!string.IsNullOrEmpty(_httpConfig.ApiKey))
            {
                headers["X-API-Key"] = _httpConfig.ApiKey;
            }

            return headers;
        }
    }
}
