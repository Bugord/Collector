using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Collector.BL.Entity;
using Collector.BL.Extentions;
using Collector.DAO.Entities;
using Collector.DAO.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Collector.BL.Authorization
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<User> _userService;

        public TokenService(IConfiguration configuration, 
            IRepository<User> userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        public async Task<object> RegisterAsync(RegisterDto entity)
        {
            var foundedUser =
                 (_userService.GetAllAsync(user => user.Username == entity.Username || user.Email == entity.Email).Result).Any();

            //if (foundedUser)
            //    throw new Exception("This username or(and) email is already in use");

            var createdBy = (await (_userService.GetAllAsync(user => user.Username == "system").Result).FirstOrDefaultAsync()).ID;
            Console.WriteLine("Created by:" + createdBy);

            var newUser = new User
            {
                Username = entity.Username,
                Email = entity.Email,
                Password = Enctypting.CreateMD5(entity.Password),
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Role = Role.User,
                CreatedBy = createdBy
            };


            await _userService.InsertAsync(newUser);

            return GenerateJwtToken(entity.Username, Role.User.ToString());
        }

        public async Task<object> LoginAsync(LoginDto entity)
        {
            var foundedUser = await (await _userService.GetAllAsync(user => user.Email == entity.Email && user.Password == Enctypting.CreateMD5(entity.Password))).FirstOrDefaultAsync();

            if(foundedUser!= null)
                return GenerateJwtToken(foundedUser.Username, foundedUser.Role.ToString());

            throw new Exception();
        }

        private object GenerateJwtToken(string login, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, login),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimsIdentity.DefaultNameClaimType, login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role),
            };


            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));
            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtIssuer"],
                claims,
                expires: expires
            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);
            return encodedJwt;
        }
    }

}
