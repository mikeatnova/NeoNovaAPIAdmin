using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeoNovaAPIAdmin.Models.Messaging
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CommentBody { get; set; }

        [Required]
        public string Username { get; set; }

        public DateTime CreatedAt { get; set; }

        [ForeignKey("PalantirMessageId")]
        public int PalantirMessageId { get; set; }
        public PalantirMessage PalantirMessage { get; set; }
    }
}
