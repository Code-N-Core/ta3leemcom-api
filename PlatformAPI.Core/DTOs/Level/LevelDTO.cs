using PlatformAPI.Core.DTOs.LevelYear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.DTOs.Level
{
    public class LevelDTO
    {
        public int LevelId { get; set; }
        public string LevelNames { get; set; }
        public List<LevelYearDTO> LevelYears { get; set; }
    }
}