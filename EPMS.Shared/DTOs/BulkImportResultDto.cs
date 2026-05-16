using System.Collections.Generic;

namespace EPMS.Shared.DTOs;

public class BulkImportResultDto
{
    public int Imported { get; set; }
    public int Rejected { get; set; }
    public List<string> Errors { get; set; } = new();
}
