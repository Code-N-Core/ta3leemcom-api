using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.Services
{
    public class AttachmentService
    {
        public string GetAttachmentType(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();

            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                    return "image";

                case ".mp4":
                case ".avi":
                case ".mov":
                case ".wmv":
                    return "video";

                case ".mp3":
                case ".wav":
                case ".aac":
                    return "audio";

                default:
                    return "unknown"; // or throw an exception for unsupported types
            }
        }
    }
}
