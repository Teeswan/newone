using System;
using System.Collections.Generic;

namespace EPMS.Domain.Entities;

public partial class AppraisalResponse
{
    public long ResponseId { get; set; }
    public int? EvalId { get; set; }
    public int? QuestionId { get; set; }
    public int? RespondentId { get; set; } 
    public string RespondentRole { get; set; } = null!;
    public string? AnswerText { get; set; }
    public int? RatingValue { get; set; }
    public bool? IsAnonymous { get; set; }
    public virtual PerformanceEvaluation? Eval { get; set; }
    public virtual AppraisalQuestion? Question { get; set; }
    public virtual Employee? Respondent { get; set; }
}
