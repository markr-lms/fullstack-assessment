using LMS.Assessment.Api.Abstractions;

namespace LMS.Assessment.Api.Entities;

public record LawFirm(
    Guid Id,
    string Name,
    string Address,
    string PhoneNumber,
    string Email,
    Guid CreatedBy) : IEntity
{
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
