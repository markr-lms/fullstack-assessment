using LMS.Assessment.Api.Abstractions;

namespace LMS.Assessment.Api.Entities;

public record Document(
    Guid Id,
    string Title,
    string Type,
    Guid LawFirmId,
    Guid UploadedBy,
    DateTime UploadedAt,
    Guid CreatedBy) : IEntity
{
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
