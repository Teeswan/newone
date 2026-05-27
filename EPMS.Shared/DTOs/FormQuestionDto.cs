namespace EPMS.Shared.DTOs;

public class FormQuestionDto
{
    public int? FormId { get; set; }
    public int? QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int? SortOrder { get; set; }
}
