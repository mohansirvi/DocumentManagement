using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DocumentManagement.Enums;

namespace DocumentManagement.Models
{
    public class IngestionRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DocumentId { get; set; }

        [ForeignKey("DocumentId")]
        public Document Document { get; set; }

        [Required]
        public string Status { get; set; } = IngestionStatus.Pending.ToString();

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }
}
