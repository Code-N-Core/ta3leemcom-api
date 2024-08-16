using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.Settings
{
    public static class FileSettings
    {
        public const string AllowedExtensions = ".jpg,.jpeg,.png,.gif,.mp4,.avi,.mov,.wmv,.mp3,.wav,.aac";
        public const int MaxFileSizeInMB = 5;
        public const int MaxFileSizeInBytes = MaxFileSizeInMB * 1024 * 1024;

      
    }
}
