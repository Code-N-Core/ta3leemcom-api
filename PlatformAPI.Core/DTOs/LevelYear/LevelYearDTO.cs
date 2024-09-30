using PlatformAPI.Core.DTOs.Groub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.DTOs.LevelYear
{
    public class LevelYearDTO
    {
        public int LevelYearId { get; set; }
        public string LevelYearName { get; set; }
        public List<LevelGroupDTO> LevelGroups { get; set; }
    }
}
