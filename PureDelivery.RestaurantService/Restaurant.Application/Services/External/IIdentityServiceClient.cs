using PureDelivery.Shared.Contracts.DTOs.Identity.Requests;
using PureDelivery.Shared.Contracts.DTOs.Identity.Responses;

namespace Restaurant.Application.Services.External
{
    public interface IIdentityServiceClient
    {
        Task<RegisterUserCredentialResult?> RegisterUserCredentialAsync(
            RegisterUserCredentialRequest request,
            CancellationToken ct = default);
    }
}
