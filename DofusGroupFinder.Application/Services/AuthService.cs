using DofusGroupFinder.Domain.DTO;
using DofusGroupFinder.Domain.Entities;
using DofusGroupFinder.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DofusGroupFinder.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _context.Accounts.AnyAsync(a => a.Pseudo == request.Pseudo))
                throw new Exception("Email already registered.");

            var account = new Account
            {
                Id = Guid.NewGuid(),
                Pseudo = request.Pseudo,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return GenerateToken(account);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Pseudo == request.Pseudo);
            if (account == null)
                throw new Exception("Invalid credentials.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, account.PasswordHash))
                throw new Exception("Invalid credentials.");

            return GenerateToken(account);
        }

        private AuthResponse GenerateToken(Account account)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpiresInMinutes"]!)),
                signingCredentials: creds
            );

            return new AuthResponse { Token = new JwtSecurityTokenHandler().WriteToken(token) };
        }
    }
}