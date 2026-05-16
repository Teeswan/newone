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
}
