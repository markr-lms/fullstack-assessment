namespace LMS.Assessment.Api.Abstractions;

public interface IEntity
{
    Guid Id { get; }
    Guid CreatedBy { get; }
    DateTime CreatedAt { get; }
}
