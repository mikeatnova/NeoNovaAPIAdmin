using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NeoNovaAPIAdmin.Models.SecurityModels
{
    [Table("CameraLocations")]
    public class CameraLocation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [ForeignKey("Location")]
        public int LocationId { get; set; }

        public Location Location { get; set; }

        [ForeignKey("Camera")]
        public int CameraId { get; set; }

        public Camera Camera { get; set; }
    }
}
