using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Collector.DAO.Entities;
using Microsoft.AspNetCore.Http;

namespace Collector.BL.Services.UploadService
{
    public interface IUploadService
    {
        Task<string> UploadFileAsync(IFormFile file, UploadType uploadType);
    }
}
