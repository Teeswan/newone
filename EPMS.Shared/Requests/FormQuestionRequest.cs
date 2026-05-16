namespace EPMS.Shared.Requests;

public class CreateFormQuestionRequest
{
    public int? FormId { get; set; }
    public int? QuestionId { get; set; }
    public int? SortOrder { get; set; }
}

public class UpdateFormQuestionRequest
{
    public int? FormId { get; set; }
    public int? QuestionId { get; set; }
    public int? SortOrder { get; set; }
}
