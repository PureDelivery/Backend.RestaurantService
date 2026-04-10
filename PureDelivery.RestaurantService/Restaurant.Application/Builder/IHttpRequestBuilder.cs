using PureDelivery.Common.Http.Models;
using PureDelivery.Shared.Contracts.DTOs.Location.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Builder
{
    public interface IHttpRequestBuilder
    {
        HttpRequestParams BuildFilterRestaurantsRequest(FilterRestaurantsByLocationRequest filterRequest);
    }
}
