namespace Inspections.Models.Entities;

public class StateRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string StateId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Points { get; set; }
    public bool Visited { get; set; }
    public double Distance { get; set; }

    // Image storage
    public string? ImageUrl { get; set; }
    public string? ImageFileName { get; set; }
    public DateTime? ImageUploadedAt { get; set; }
}
