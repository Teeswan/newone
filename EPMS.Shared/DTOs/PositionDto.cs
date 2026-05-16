namespace EPMS.Shared.DTOs;

public class PositionDto
{
    public int PositionId { get; set; }
    public string PositionTitle { get; set; } = null!;
    public string? LevelId { get; set; }
    public string? LevelName { get; set; }
}
