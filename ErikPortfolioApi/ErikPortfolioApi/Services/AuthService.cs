using ErikPortfolioApi.configuration;
using ErikPortfolioApi.exceptions;
using ErikPortfolioApi.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ErikPortfolioApi.Services
{
    public class AuthService
    {
        private readonly JwtConfig _jwtConfig;
        private readonly PasswordHasher<string> _passwordHasher;
        private readonly AuthRepository _authRepository;

        public AuthService(IOptionsMonitor<JwtConfig> optionsMonitor, AuthRepository authRepository)
        {
            _jwtConfig = optionsMonitor.CurrentValue;
            _passwordHasher = new PasswordHasher<string>();
            _authRepository = authRepository;
        }

        public async Task AddLogin(string username, string password)
        {
            await _authRepository.AddLogin(username, _passwordHasher.HashPassword(username, password));
        }

        public async Task<string> Login(string username, string password)
        {
            var hashedPassword = await _authRepository.GetHashedPassword(username);

            var result = _passwordHasher.VerifyHashedPassword(username, hashedPassword, password);

            switch (result)
            {
                case PasswordVerificationResult.Success:
                    return GenerateJwtToken(username);
                default:
                    throw new FailedLoginException("Invalid password");
            }
        }

        private string GenerateJwtToken(string username)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                }),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }
}
