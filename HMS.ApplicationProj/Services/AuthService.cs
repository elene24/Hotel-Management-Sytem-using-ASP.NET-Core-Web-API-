using System;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HMS.ApplicationProj.DTOS;
using HMS.DomainProj.Entities;
using HMS.InfrastructureProj.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HMS.ApplicationProj.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            if (dto.Role == "Manager")
                throw new ArgumentException("Managers must be created through /api/hotels/{hotelId}/managers");

            if (dto.Role != "Guest" && dto.Role != "Admin")
                throw new ArgumentException("Role must be either Guest or Admin.");

            if (string.IsNullOrWhiteSpace(dto.FirstName))
                throw new ArgumentException("FirstName is required.");

            if (string.IsNullOrWhiteSpace(dto.LastName))
                throw new ArgumentException("LastName is required.");

            if (string.IsNullOrWhiteSpace(dto.PersonalNumber))
                throw new ArgumentException("PersonalNumber is required.");

            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                throw new ArgumentException("PhoneNumber is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Password is required.");

            if (dto.Role == "Admin" && string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email is required for Admin registration.");

            var guestExists = await _context.Guests
                .AnyAsync(g => g.PersonalNumber == dto.PersonalNumber || g.PhoneNumber == dto.PhoneNumber);

            if (guestExists)
                return false;

            if (dto.Role == "Admin")
            {
                var adminExists = await _context.Managers
                    .AnyAsync(m => m.Email == dto.Email || m.PersonalNumber == dto.PersonalNumber);

                if (adminExists)
                    return false;
            }

            var passwordHash = HashPassword(dto.Password);

            if (dto.Role == "Guest")
            {
                var guest = new Guest
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    PersonalNumber = dto.PersonalNumber,
                    PhoneNumber = dto.PhoneNumber,
                    PasswordHash = passwordHash,
                    Role = "Guest"
                };

                _context.Guests.Add(guest);
            }
            else
            {
                var admin = new Manager
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email!,
                    PersonalNumber = dto.PersonalNumber,
                    PhoneNumber = dto.PhoneNumber,
                    PasswordHash = passwordHash,
                    Role = "Admin",
                    HotelId = null
                };

                _context.Managers.Add(admin);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.EmailOrPersonalNumber))
                throw new ArgumentException("EmailOrPersonalNumber is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Password is required.");

            var guest = await _context.Guests
                .FirstOrDefaultAsync(g => g.PersonalNumber == dto.EmailOrPersonalNumber);

            if (guest != null && VerifyPassword(dto.Password, guest.PasswordHash))
            {
                return new AuthResponseDto
                {
                    Token = GenerateJwtToken(guest.PersonalNumber, guest.Role),
                    Role = guest.Role
                };
            }

            var manager = await _context.Managers
                .FirstOrDefaultAsync(m => m.Email == dto.EmailOrPersonalNumber);

            if (manager != null && VerifyPassword(dto.Password, manager.PasswordHash))
            {
                return new AuthResponseDto
                {
                    Token = GenerateJwtToken(manager.Email, manager.Role),
                    Role = manager.Role
                };
            }

            return null;
        }

        private string GenerateJwtToken(string identifier, string role)
        {
            var jwtKey = _config["Jwt:Key"] ?? "ThisIsASuperSecretJwtKey1234567890";
            var jwtIssuer = _config["Jwt:Issuer"] ?? "HMS_API";
            var jwtAudience = _config["Jwt:Audience"] ?? "HMS_CLIENT";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, identifier),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            return Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(password)));
        }

        private bool VerifyPassword(string password, string hash)
        {
            var checkHash = HashPassword(password);
            return checkHash == hash;
        }
    }
}