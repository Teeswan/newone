namespace EPMS.Shared.DTOs;

public class AppraisalResponseDto
{
    public long ResponseId { get; set; }
    public int? EvalId { get; set; }
    public int? QuestionId { get; set; }
    public int? RespondentId { get; set; }
    public int? RespondentEmployeeId { get; set; }
    public string? RespondentRole { get; set; }
    public string? AnswerText { get; set; }
    public int? RatingValue { get; set; }
    public bool? IsAnonymous { get; set; }

    // Navigation info for reports
    public string? QuestionText { get; set; }
    public string? QuestionCategory { get; set; }
    public string? RespondentName { get; set; }
    public string? RespondentPosition { get; set; }
    public string? RespondentDepartment { get; set; }
    public int? SortOrder { get; set; }
}
