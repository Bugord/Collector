﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Collector.BL.Exceptions;
using Collector.BL.Extentions;
using Collector.BL.Models.Authorization;
using Collector.BL.Services.EmailService;
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
        private readonly IRepository<EmailConfirmation> _emailConfirmationRepository;
        private readonly IEmailService _emailService;

        public TokenService(IConfiguration configuration,
            IRepository<User> userRepository, IRepository<EmailConfirmation> emailConfirmationRepository,
            IEmailService emailService)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _emailConfirmationRepository = emailConfirmationRepository;
            _emailService = emailService;
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

            if (bool.Parse(_configuration["EmailConfirmation"]))
                await SendConfrimationEmail(newUser, system.Id);

            return new AuthorizationReturnDTO
            {
                Token = GenerateJwtToken(model.Username, Role.User.ToString(), newUser.Id),
                User = newUser.UserToUserReturnDTO()
            };
        }

        public async Task SendConfrimationEmail(User user, long systemId)
        {
            var encryptedEmail = HttpUtility.UrlEncode((user.Email + "|" + DateTime.UtcNow).CreateMd5());
            await _emailService.SendEmailAsync(user.Email, "Email confirmation",
                $"To confirm email follow this <a href={_configuration["FrontendAdress"]}/confirmEmail/{encryptedEmail}>link</a> ");

            var newEmailConfirmation = new EmailConfirmation
            {
                User = user,
                CreatedBy = systemId,
                VerificationToken = encryptedEmail
            };
            await _emailConfirmationRepository.InsertAsync(newEmailConfirmation);
            throw new Exception("Check your email and confirm registration");
        }

        public async Task<AuthorizationReturnDTO> LoginAsync(LoginDTO model)
        {
            var passwordHash = model.Password.CreateMd5();
            var foundedUser = await _userRepository.GetFirstAsync(user =>
                user.Email == model.Email && user.Password == passwordHash);
            if (foundedUser == null)
                throw new AuthenticationFailException("Wrong login or(and) password");
            if (bool.Parse(_configuration["EmailConfirmation"]) && !foundedUser.Confirmed)
                throw new AuthenticationFailException("you have not confirmed your email");


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