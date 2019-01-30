using System;
using System.Data.SqlTypes;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Collector.BL.Services.EmailService;
using Collector.BL.Exceptions;
using Collector.BL.Extentions;
using Collector.BL.Models.Authorization;
using Collector.BL.Services.UploadService;
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
        private readonly IUploadService _uploadService;
        private readonly IRepository<EmailConfirmation> _emailConfirmationRepository;
        private readonly IRepository<PasswordReset> _passwordResetRepository;

        public UserService(IRepository<User> userRepository, IConfiguration configuration,
            IEmailService emailService, IHttpContextAccessor httpContextAccessor,
            IRepository<EmailConfirmation> emailConfirmationRepository,
            IRepository<PasswordReset> passwordResetRepository, IUploadService uploadService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _emailConfirmationRepository = emailConfirmationRepository;
            _passwordResetRepository = passwordResetRepository;
            _uploadService = uploadService;
        }

        public async Task<UserReturnDTO> GetUserInfoAsync(long? id)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var user = await _userRepository.GetByIdAsync(id ?? ownerId);
            return user.UserToUserReturnDTO();
        }

        public async Task ConfirmEmailAsync(string token)
        {
            var confirmationToApprove = await (await _emailConfirmationRepository.GetAllAsync(
                confirmation => confirmation.VerificationToken == token && !confirmation.Used,
                confirmation => confirmation.User)).FirstOrDefaultAsync();

            if (confirmationToApprove == null)
                throw new ArgumentException();

            confirmationToApprove.User.Confirmed = true;
            await _userRepository.UpdateAsync(confirmationToApprove.User);

            confirmationToApprove.ConfirmationTime = DateTime.UtcNow;
            confirmationToApprove.Used = true;
            await _emailConfirmationRepository.UpdateAsync(confirmationToApprove);
        }

        public async Task ChangePasswordAsync(ChangePasswordDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var user = await _userRepository.GetByIdAsync(ownerId);
            if (model.OldPassword.CreateMd5() != user.Password)
                throw new ArgumentException("Wrong old password");
            user.Password = model.NewPassword.CreateMd5();

            await _emailService.SendEmailAsync(user.Email, "Password changed",
                "Your password was changed. If you didn't change your password, please, contact us.");

            await _userRepository.UpdateAsync(user);
        }


        public async Task<UserReturnDTO> ChangeProfileAsync(ChangeProfileDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var oldUser = await _userRepository.GetByIdAsync(ownerId);

            if (model.AvatarFile != null && model.AvatarFile.Length != 0)
            {
                var avatarUrl = await _uploadService.UploadFileAsync(model.AvatarFile, UploadType.Avatar);
                oldUser.AratarUrl = avatarUrl;

                //var fileName = oldUser.Username + "_" + DateTime.UtcNow.ToFileTimeUtc() + Path.GetExtension(model.AvatarFile.FileName);
                //var path = Path.Combine(
                //    Directory.GetCurrentDirectory(), "wwwroot", _configuration["ImagesPath"],oldUser.Username, fileName
                //);
                //Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", _configuration["ImagesPath"], oldUser.Username));
                //using (var stream = new FileStream(path, FileMode.Create))
                //{
                //    await model.AvatarFile.CopyToAsync(stream);
                //}

                //oldUser.AratarUrl = Path.Combine("images/" , oldUser.Username, fileName);
            }

            var emailExist =
                await (await _userRepository.GetAllAsync(user => user.Email.Equals(model.Email, StringComparison.CurrentCultureIgnoreCase) && user.Id != ownerId))
                    .AnyAsync();
            if (emailExist)
                throw new AlreadyExistsException("This email already exist");

            oldUser.UpdateUser(model);

            await _userRepository.UpdateAsync(oldUser);

            return oldUser.UserToUserReturnDTO();
        }


        public async Task ResetPasswordAsync(string email)
        {
            var userToReset = await _userRepository.GetFirstAsync(user => user.Email == email);
            if (userToReset == null)
                throw new SqlNullValueException("This email does not exist");

            var tokenToEncrypt = email + DateTime.UtcNow;
            var encryptedToken = tokenToEncrypt.CreateMd5();
            encryptedToken = HttpUtility.UrlEncode(encryptedToken);

            var oldReset =
                await _passwordResetRepository.GetFirstAsync(reset => reset.User.Email == email && !reset.Used);
            if (oldReset != null)
            {
                oldReset.Used = true;
                oldReset.ResetDate = DateTime.UtcNow;
                await _passwordResetRepository.UpdateAsync(oldReset);
            }

            var systemUser = await _userRepository.GetFirstAsync(user => user.Username == "system");
            if (systemUser == null)
                throw new ServerFailException("Internal server error");

            var passwordReset = new PasswordReset
            {
                CreatedBy = systemUser.Id,
                ExpirationTime = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["ResetTokenExpireMinutes"])),
                VerificationToken = encryptedToken,
                User = userToReset
            };
            await _passwordResetRepository.InsertAsync(passwordReset);

            await _emailService.SendEmailAsync(email, "Password reset",
                $"To reset password follow this <a href={_configuration["FrontendAdress"]}/resetPassword/{encryptedToken}>link</a> " +
                "<br>" +
                $" Link will expire in {_configuration["ResetTokenExpireMinutes"]} minutes."
            );
        }

        public async Task ResetPasswordTokenAsync(ResetPasswordDTO model)
        {
            var passwordReset =
                await _passwordResetRepository.GetFirstAsync(
                    reset => reset.VerificationToken == model.Token && !reset.Used,
                    reset => reset.User);

            if (passwordReset == null || passwordReset.Used)
                throw new SecurityTokenException("Token is not valid");

            if (passwordReset.ExpirationTime < DateTime.UtcNow)
                throw new SecurityTokenInvalidLifetimeException("The token has expired");

            if (passwordReset.User == null)
                throw new SqlNullValueException("No users found with this email address");


            await _emailService.SendEmailAsync(passwordReset.User.Email, "Password changed",
                "Your password was reset. If you didn't reset your password, please, contact us.");

            passwordReset.User.Password = model.Password.CreateMd5();
            await _userRepository.UpdateAsync(passwordReset.User);

            passwordReset.Used = true;
            passwordReset.ResetDate = DateTime.UtcNow;

            await _passwordResetRepository.UpdateAsync(passwordReset);
        }
    }
}