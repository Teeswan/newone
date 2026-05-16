namespace EPMS.Shared.Requests;

public class CreateAppraisalFormRequest
{
    public string? FormName { get; set; }
    public bool? IsActive { get; set; }
}

public class UpdateAppraisalFormRequest
{
    public string? FormName { get; set; }
    public bool? IsActive { get; set; }
}
