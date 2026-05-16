namespace EPMS.Shared.Requests;

public class CreateAppraisalQuestionRequest
{
    public string QuestionText { get; set; } = null!;
    public string? Category { get; set; }
    public bool? IsRequired { get; set; }
}

public class UpdateAppraisalQuestionRequest
{
    public string QuestionText { get; set; } = null!;
    public string? Category { get; set; }
    public bool? IsRequired { get; set; }
}
