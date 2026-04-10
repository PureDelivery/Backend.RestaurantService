using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Exceptions
{
    public class RestaurantException : Exception
    {
        public string ErrorCode { get; }
        public string UserMessage { get; }
        public HttpStatusCode StatusCode { get; }

        public RestaurantException(
            string errorCode,
            string userMessage,
            HttpStatusCode statusCode = HttpStatusCode.OK,
            string? internalMessage = null,
            Exception? innerException = null)
            : base(internalMessage ?? userMessage, innerException)
        {
            ErrorCode = errorCode;
            UserMessage = userMessage;
            StatusCode = statusCode;
        }

        public static RestaurantException NotFound(Guid restaurantId) =>
            new("RESTAURANT_NOT_FOUND",
                "Restaurant not found",
                HttpStatusCode.NotFound,
                $"Restaurant with ID {restaurantId} not found");

        public static RestaurantException ValidationError(string userMessage, string? internalMessage = null) =>
            new("VALIDATION_ERROR",
                userMessage,
                HttpStatusCode.BadRequest,
                internalMessage);

        // Обновленный метод с дополнительными параметрами
        public static RestaurantException LocationServiceError(string errorCode, string userMessage, string? internalMessage = null) =>
            new($"LOCATION_SERVICE_{errorCode}",
                userMessage,
                HttpStatusCode.OK,
                internalMessage);

        public static RestaurantException NotDeliverable() =>
            new("RESTAURANT_NOT_DELIVERABLE",
                "This restaurant doesn't deliver to your location",
                HttpStatusCode.OK);

        public static RestaurantException InternalError(string internalMessage) =>
            new("INTERNAL_ERROR",
                "Service temporarily unavailable. Please try again later.",
                HttpStatusCode.OK,
                internalMessage);

        public static RestaurantException EmptyResult(string operation) =>
            new("EMPTY_RESULT",
                "No results found",
                HttpStatusCode.OK,
                $"No results found for operation: {operation}");
    }
}
