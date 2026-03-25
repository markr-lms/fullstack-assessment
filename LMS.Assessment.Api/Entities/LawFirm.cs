using LMS.Assessment.Api.Abstractions;

namespace LMS.Assessment.Api.Entities;

public record LawFirm(
    string Id,
    string Name,
    string Address,
    string PhoneNumber,
    string Email) : IEntity;
