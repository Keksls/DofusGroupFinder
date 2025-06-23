using Zaapix.Domain.DTO.Requests;
using Zaapix.Domain.DTO.Responses;

namespace Zaapix.Application.Services
{
    public interface IListingService
    {
        Task<List<PublicListingResponse>> GetMyListingsAsync(Guid accountId);
        Task<ListingResponse> CreateListingAsync(Guid accountId, CreateListingRequest request);
        Task<ListingResponse> UpdateListingAsync(Guid accountId, Guid listingId, UpdateListingRequest request);
        Task DeleteListingAsync(Guid accountId, Guid listingId);
        Task<List<PublicListingResponse>> GetPublicListingsAsync(string server);
        Task<List<PublicListingResponse>> SearchPublicListingsAsync(PublicListingSearchRequest request);
        Task<PublicListingResponse?> GetPublicListingByIdAsync(Guid listingId);
    }
}