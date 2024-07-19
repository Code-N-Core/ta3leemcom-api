﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.Models
{
    public class Choose
    {
        [Key]
        public int Id { get; set; }
        [Required,MinLength(1),MaxLength(1000)]
        public string Content { get; set; }
        [Required]
        public int QuestionId { get; set; }
        public virtual Question Question { get; set; }
    }
}