using System.ComponentModel.DataAnnotations.Schema;

namespace Asm.Models
{
    [NotMapped]
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
