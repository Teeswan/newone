using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPMS.Domain.Entities;

[Table("FormQuestions")]
public partial class FormQuestion
{
    public int FormId { get; set; }
    public int QuestionId { get; set; }
    public int SortOrder { get; set; }

    public virtual ApplicationForm Form { get; set; } = null!;
    public virtual AppraisalQuestion Question { get; set; } = null!;
}