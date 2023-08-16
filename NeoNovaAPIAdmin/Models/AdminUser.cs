using System.ComponentModel.DataAnnotations;

namespace NeoNovaAPIAdmin.Models
{
    public class AdminUser
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
