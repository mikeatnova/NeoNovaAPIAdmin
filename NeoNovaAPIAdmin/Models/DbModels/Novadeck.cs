﻿using System.ComponentModel.DataAnnotations;

namespace NeoNovaAPIAdmin.Models.DbModels
{
    public class Novadeck
    {
        [Key]
        public int Id { get; set; }
        public string? Term { get; set; }
        public string? TermDefinition { get; set; }
        public int IsPinned { get; set; }

    }
}
