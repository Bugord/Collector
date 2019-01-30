using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Collector.BL.Extentions;
using Collector.DAO.Entities;
using Collector.DAO.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Collector.BL.Services.UploadService
{
    public class UploadService : IUploadService
    {
        private readonly IRepository<Upload> _uploadRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UploadService(IRepository<Upload> uploadRepository, IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor, IRepository<User> userRepository)
        {
            _uploadRepository = uploadRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        public async Task<string> UploadFileAsync(IFormFile file, UploadType uploadType)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var ownerUser = await _userRepository.GetByIdAsync(ownerId);

            string folderName;

            switch (uploadType)
            {
                case UploadType.Avatar:
                    folderName = _configuration["ImagesPath"];
                    break;
                case UploadType.Chat:
                    folderName = _configuration["ChatUploadPath"];
                    break;
                default:
                    folderName = _configuration["ChatUploadPath"];
                    break;
            }

            if (file != null && file.Length != 0)
            {
                var fileName = (ownerUser.Username + "_" + DateTime.UtcNow.ToFileTimeUtc()).CreateMd5() +
                               Path.GetExtension(file.FileName);
                var fullFolderPath = Path.Combine(
                     @"./wwwroot", folderName, ownerUser.Username
                );
                var relativeFilePath = Path.Combine(
                    folderName, ownerUser.Username, fileName
                );

                var filePath = Path.Combine(
                    fullFolderPath, fileName
                );
                Directory.CreateDirectory(fullFolderPath);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var upload = new Upload
                {
                    CreatedBy = ownerId,
                    Extention = Path.GetExtension(file.FileName),
                    Name = fileName,
                    OriginalName = Path.GetFileNameWithoutExtension(file.FileName),
                    Size = file.Length,
                    Type = uploadType,
                    Path = relativeFilePath
                };

                await _uploadRepository.InsertAsync(upload);

                return relativeFilePath;
            }

            throw new FileLoadException("File is empty");
        }
    }
}