namespace EPMS.Shared.DTOs;

public class AppraisalQuestionDto
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = null!;
    public string? Category { get; set; }
    public bool? IsRequired { get; set; }
}
