using DofusGroupFinder.Domain.DTO;

namespace DofusGroupFinder.Application.Services
{
    public interface IListingService
    {
        Task<List<ListingResponse>> GetMyListingsAsync(Guid accountId);
        Task<ListingResponse> CreateListingAsync(Guid accountId, CreateListingRequest request);
        Task<ListingResponse> UpdateListingAsync(Guid accountId, Guid listingId, UpdateListingRequest request);
        Task DeleteListingAsync(Guid accountId, Guid listingId);
        Task<List<PublicListingResponse>> GetPublicListingsAsync(string server);
        Task<List<PublicListingResponse>> SearchPublicListingsAsync(string server, PublicListingSearchRequest request);
    }
}