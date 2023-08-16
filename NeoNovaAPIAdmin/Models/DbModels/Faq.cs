﻿using System.ComponentModel.DataAnnotations;

namespace NeoNovaAPIAdmin.Models.DbModels
{
    public class Faq
    {
        [Key]
        public int Id { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public string? State { get; set; }
        public bool IsPinned { get; set; }
    }
}
