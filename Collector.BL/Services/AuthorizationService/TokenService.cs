using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Collector.BL.Exceptions;
using Collector.BL.Extentions;
using Collector.BL.Models.Authorization;
using Collector.DAO.Entities;
using Collector.DAO.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Collector.BL.Services.AuthorizationService
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<User> _userRepository;

        public TokenService(IConfiguration configuration,
            IRepository<User> userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }


        public async Task<AuthorizationReturnDTO> RegisterAsync(RegisterDTO model)
        {
            var foundedUser = await _userRepository.ExistsAsync(user =>
                user.Username == model.Username || user.Email == model.Email);
            if (foundedUser)
                throw new AlreadyExistsException("This username or(and) email is already in use");

            var system = await _userRepository.GetFirstAsync(user => user.Username == "system");
            if (system == null)
                throw new ServerFailException("System user not found");
            var createdBy = system.Id;

            var newUser = model.RegisterDTOToUser(createdBy);
            newUser = await _userRepository.InsertAsync(newUser);

            return new AuthorizationReturnDTO
            {
                Token = GenerateJwtToken(model.Username, Role.User.ToString(), newUser.Id),
                User = newUser.UserToUserReturnDTO()
            };
        }

        public async Task<AuthorizationReturnDTO> LoginAsync(LoginDTO model)
        {
            var passwordHash = model.Password.CreateMd5();
            var foundedUser = await _userRepository.GetFirstAsync(user =>
                user.Email == model.Email && user.Password == passwordHash);
            if (foundedUser == null)
                throw new AuthenticationFailException("Wrong login or(and) password");

            return new AuthorizationReturnDTO
            {
                Token = GenerateJwtToken(foundedUser.Username, foundedUser.Role.ToString(), foundedUser.Id),
                User = foundedUser.UserToUserReturnDTO()
            };
        }

        private string GenerateJwtToken(string login, string role, long id)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role),
                new Claim(ClaimTypes.NameIdentifier, id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));
            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtAudience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);
            return encodedJwt;
        }
    }
}