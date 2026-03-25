using LMS.Assessment.Api.Abstractions;

namespace LMS.Assessment.Api.Entities;

public record Document(
    string Id,
    string Title,
    string Type,
    string LawFirmId,
    string UploadedBy,
    DateTime UploadedAt) : IEntity;
