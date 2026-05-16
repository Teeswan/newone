namespace EPMS.Shared.Requests;

public class CreateAppraisalResponseRequest
{
    public int? EvalId { get; set; }
    public int? QuestionId { get; set; }
    public int? RespondentId { get; set; }
    public int? RespondentEmployeeId { get; set; }
    public string? RespondentRole { get; set; }
    public string? AnswerText { get; set; }
    public int? RatingValue { get; set; }
    public bool? IsAnonymous { get; set; }
}

public class UpdateAppraisalResponseRequest
{
    public int? EvalId { get; set; }
    public int? QuestionId { get; set; }
    public int? RespondentId { get; set; }
    public int? RespondentEmployeeId { get; set; }
    public string? RespondentRole { get; set; }
    public string? AnswerText { get; set; }
    public int? RatingValue { get; set; }
    public bool? IsAnonymous { get; set; }
}
