using System;
using System.Data.SqlTypes;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Collector.BL.Services.EmailService;
using Collector.BL.Exceptions;
using Collector.BL.Extentions;
using Collector.BL.Models.Authorization;
using Collector.DAO.Entities;
using Collector.DAO.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Collector.BL.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public UserService(IRepository<User> userRepository, IConfiguration configuration,
            IEmailService emailService, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task ChangePasswordAsync(ChangePasswordDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var user = await _userRepository.GetByIdAsync(ownerId);
            if (model.OldPassword.CreateMd5() != user.Password)
                throw new Exception("Wrong old password");
            user.Password = model.NewPassword.CreateMd5();
            await _emailService.SendEmailAsync(user.Email, "Password changed",
                "Your password was changed. If you didn't change your password, please, contact us.");
            await _userRepository.UpdateAsync(user);
        }


        public async Task<UserReturnDTO> ChangeProfileAsync(ChangeProfileDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);
            var oldUser = await _userRepository.GetByIdAsync(ownerId);

            var userNameExist =
                await (await _userRepository.GetAllAsync(user => user.Username == model.Username && user.Id != ownerId))
                    .AnyAsync();
            if (userNameExist)
                throw new AlreadyExistsException("This username already exist");

            var emailExist =
                await (await _userRepository.GetAllAsync(user => user.Email == model.Email && user.Id != ownerId))
                    .AnyAsync();
            if (emailExist)
                throw new AlreadyExistsException("This email already exist");

            oldUser.UpdateUser(model);

            await _userRepository.UpdateAsync(oldUser);

            return oldUser.UserToUserReturnDTO();
        }


        public async Task ResetPasswordAsync(string email)
        {
            var isExsist = await _userRepository.ExistsAsync(user => user.Email == email);
            if (!isExsist)
                throw new SqlNullValueException("This email does not exist");
            var tokenToEncrypt = email + "|" +
                                 DateTime.UtcNow.AddMinutes(int.Parse(_configuration["ResetTokenExpireMinutes"]));
            var encryptedText = Enctypting.Encrypt(tokenToEncrypt, _configuration["EncryptionKey"], true);
            encryptedText = HttpUtility.UrlEncode(encryptedText);
            await _emailService.SendEmailAsync(email, "Password reset",
                $"To reset password follow this <a href=http://localhost:3000/resetPassword/{encryptedText}>link</a> " +
                "<br>" +
                $" Link will expire in {_configuration["ResetTokenExpireMinutes"]} minutes."
            );
        }

        public async Task ResetPasswordTokenAsync(ResetPasswordDTO model)
        {
            var decryptedToken =
                Enctypting.Decrypt(HttpUtility.UrlDecode(model.Token), _configuration["EncryptionKey"], true);
            var email = decryptedToken.Split('|')[0];
            var expireTime = DateTime.Parse(decryptedToken.Split('|')[1]);

            if (DateTime.UtcNow > expireTime)
                throw new SecurityTokenInvalidLifetimeException("Token is expired");
            var oldUser =
                await _userRepository.GetFirstAsync(user => user.Email == email);
            if (oldUser == null)
                throw new SqlNullValueException("No users found with this email address");

            await _emailService.SendEmailAsync(oldUser.Email, "Password changed",
                "Your password was reset. If you didn't reset your password, please, contact us.");

            oldUser.Password = model.Password.CreateMd5();
            await _userRepository.UpdateAsync(oldUser);
        }
    }
}