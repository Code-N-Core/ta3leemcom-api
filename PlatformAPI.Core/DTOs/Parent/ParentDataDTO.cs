﻿using PlatformAPI.Core.DTOs.Child;

namespace PlatformAPI.Core.DTOs.Parent
{
    public class ParentDataDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public IEnumerable<ParentChildDTO> Chidren { get; set; }
    }
}