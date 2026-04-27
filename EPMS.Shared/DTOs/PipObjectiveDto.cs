namespace EPMS.Shared.DTOs
{
    public class PipObjectiveDto
    {
        public int ObjectiveId { get; set; }
        public int PipId { get; set; }
        public string ObjectiveDescription { get; set; } = string.Empty;
        public string? SuccessCriteria { get; set; }
        public bool IsAchieved { get; set; }
        public string? ReviewComments { get; set; }
    }
}
