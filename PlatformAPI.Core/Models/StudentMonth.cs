using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.Models
{
    public class StudentMonth
    {
        public int StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; }
        public int MonthId { get; set; }
        public virtual Month Month { get; set; }
        [Required]
        public bool Pay { get; set; }
    }
}
