using Inspections.Data;
using Inspections.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inspections.API.Controllers;

[ApiController]
[Route("api/inspection-records/{recordId:int}/image")]
public class InspectionImagesController(InspectionDbContext db) : ControllerBase
{
    private const int MaxImageBytes = 10 * 1024 * 1024;

    [HttpPost("upload")]
    [RequestSizeLimit(MaxImageBytes + 65536)]
    public async Task<IActionResult> Upload(int recordId, IFormFile file, CancellationToken token)
    {
        if (file.Length is <= 0 or > MaxImageBytes)
            return BadRequest("Select an image up to 10 MB.");

        if (!await db.Inspections.AnyAsync(item => item.Id == recordId, token))
            return NotFound();

        using var buffer = new MemoryStream((int)file.Length);
        await file.CopyToAsync(buffer, token);
        var content = buffer.ToArray();
        var mimeType = GetImageType(content);
        if (mimeType is null)
            return BadRequest("Select a PNG, JPEG, GIF, or WebP image.");

        var image = new InspectionImage
        {
            InspectionId = recordId,
            BlobName = Guid.NewGuid().ToString("N"),
            ContentType = mimeType,
            Content = content
        };
        db.InspectionImages.Add(image);
        await db.SaveChangesAsync(token);
        return CreatedAtAction(nameof(Download), new { recordId, fileName = image.BlobName },
            new { image.BlobName });
    }

    [HttpGet]
    public async Task<IActionResult> List(int recordId, CancellationToken token)
    {
        var names = await db.InspectionImages.Where(image => image.InspectionId == recordId)
            .OrderBy(image => image.Id).Select(image => new { image.BlobName }).ToListAsync(token);
        return Ok(names);
    }

    [HttpGet("download")]
    public async Task<IActionResult> Download(int recordId, string fileName, CancellationToken token)
    {
        var image = await db.InspectionImages.AsNoTracking().FirstOrDefaultAsync(
            image => image.InspectionId == recordId && image.BlobName == fileName, token);
        Response.Headers.XContentTypeOptions = "nosniff";
        return image is null ? NotFound() : File(image.Content, image.ContentType);
    }

    private static string? GetImageType(ReadOnlySpan<byte> content)
    {
        if (content.StartsWith(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 })) return "image/png";
        if (content.StartsWith(new byte[] { 255, 216, 255 })) return "image/jpeg";
        if (content.StartsWith("GIF87a"u8) || content.StartsWith("GIF89a"u8)) return "image/gif";
        if (content.Length >= 12 && content.StartsWith("RIFF"u8) && content[8..].StartsWith("WEBP"u8))
            return "image/webp";
        return null;
    }
}
