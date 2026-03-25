namespace LMS.Assessment.Api.Abstractions;

public interface IEntity
{
    string Id { get; }
    DateTime CreatedAt { get; }
}
