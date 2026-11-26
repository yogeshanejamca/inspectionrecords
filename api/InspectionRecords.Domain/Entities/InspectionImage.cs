namespace Inspections.Models.Entities;

public class InspectionImage
{
    public int Id { get; set; }
    public int InspectionId { get; set; }
    public string BlobName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public byte[] Content { get; set; } = [];
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public Inspection? Inspection { get; set; }
}
