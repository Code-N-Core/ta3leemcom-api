using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.Interfaces
{
    public interface IImageService
    {
        string SaveImage(IFormFile imageFile, string folderPath);
        void DeleteImage(string imagePath);
    }
}
