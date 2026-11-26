namespace Inspections.Models.DTOs;

public record StateRecordDto(
    Guid Id,
    string Name,
    string Icon,
    string Description,
    int Points,
    bool Visited,
    double Distance,
    string? ImageUrl,
    string? ImageFileName,
    DateTime? ImageUploadedAt);

public record CreateStateRecordRequest(
    string StateId,
    string Name,
    string Icon,
    string Description,
    int Points,
    bool Visited,
    double Distance);

public record UpdateStateRecordRequest(
    string StateId,
    string Name,
    string Icon,
    string Description,
    int Points,
    bool Visited,
    double Distance);

public record StatePlaceDto(
    string Id,
    string Name,
    string Icon,
    string Description,
    int Points,
    bool Visited,
    double Distance);

public record StateDataDto(
    string Id,
    string Name,
    string Emoji,
    double WalkingDistance,
    int TotalPoints,
    IReadOnlyList<StatePlaceDto> Places);

public record StateMapDto(
    string Id,
    string Name,
    string Icon,
    string X,
    string Y,
    string Description,
    string Color);
