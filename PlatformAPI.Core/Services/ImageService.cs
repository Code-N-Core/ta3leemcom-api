using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PlatformAPI.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.Services
{
    public class ImageService : IImageService
    {
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _webHostEnvironment;

        public ImageService(IHostingEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string SaveImage(IFormFile imageFile, string folderPath)
        {
            if (imageFile == null || imageFile.Length == 0) return null;

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);
            Directory.CreateDirectory(uploadsFolder); // Ensure the folder exists

            var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(fileStream);
            }

            return Path.Combine(folderPath, uniqueFileName).Replace("\\", "/"); // Return relative path
        }

        public void DeleteImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
