namespace DofusGroupFinder.Domain.DTO.Requests
{
    public class LoginRequest
    {
        public string Pseudo { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}