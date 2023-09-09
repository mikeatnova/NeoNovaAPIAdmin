﻿using System;

using System.ComponentModel.DataAnnotations;

namespace NeoNovaAPIAdmin.Models.DbModels
{
    public class WholesaleBugMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string MessageBody { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public DateTime DateTimestamp { get; set; }

        [Required]
        [Range(1, 5)]
        public int UrgencyRating { get; set; }
    }
}