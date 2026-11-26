using System.ComponentModel.DataAnnotations;

namespace Inspections.Models.DTOs;

public record InspectionDto(
    int Id,
    int? TemplateId,
    string Title,
    string? Description,
    string Status,
    DateTime ScheduledAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CreateInspectionRequest(
    [Required, StringLength(200)] string Title,
    [StringLength(2000)] string? Description,
    DateTime ScheduledAt,
    int? TemplateId);

public record UpdateInspectionRequest(
    [Required, StringLength(200)] string Title,
    [StringLength(2000)] string? Description,
    [Required, RegularExpression("Draft|Scheduled|InProgress|Completed|Cancelled|Paused")] string Status,
    DateTime ScheduledAt);
