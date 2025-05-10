using System.ComponentModel.DataAnnotations;
using DocumentManagement.Enums;

namespace DocumentManagement.Models.DTO
{
    public class IngestionStatusUpdateDto
    {
        [Required]
        public IngestionStatus Status { get; set; }
    }
}
