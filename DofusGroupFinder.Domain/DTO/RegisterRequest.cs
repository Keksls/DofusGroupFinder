namespace DofusGroupFinder.Domain.DTO
{
    public class RegisterRequest
    {
        public string Pseudo { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}