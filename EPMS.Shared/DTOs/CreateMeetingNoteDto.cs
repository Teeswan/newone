using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Shared.DTOs
{
    public class CreateMeetingNoteDto
    {
        [Required(ErrorMessage = "Note text is required.")]
        [MaxLength(1000)]
        public string NoteText { get; set; } = string.Empty;
        public bool IsPrivate { get; set; }
    }
}
