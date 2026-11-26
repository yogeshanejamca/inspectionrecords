namespace Inspections.Models.Entities;

public class InspectionPause
{
    public int Id { get; set; }
    public int InspectionId { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }
    public TimeSpan? Duration { get; set; }
    public string? Reason { get; set; }
}
