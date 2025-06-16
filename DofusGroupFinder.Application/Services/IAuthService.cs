using DofusGroupFinder.Domain.DTO.Requests;

namespace DofusGroupFinder.Application.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}