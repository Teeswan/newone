using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Shared.DTOs
{
    public class BulkScheduleDto
    {
        [Required]
        public List<int> EmployeeIds { get; set; } = new();
        public DateTime InitialStartTime { get; set; }
        public int DurationMinutes { get; set; }
        public int BufferGapMinutes { get; set; }
    }
}
