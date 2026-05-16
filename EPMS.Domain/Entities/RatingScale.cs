using System;
using System.Collections.Generic;

namespace EPMS.Domain.Entities;

public partial class RatingScale
{
    public int RatingLevel { get; set; }

    public string? Label { get; set; }

    public string? Description { get; set; }
}
