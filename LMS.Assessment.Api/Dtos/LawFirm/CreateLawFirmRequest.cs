using LMS.Assessment.Api.Entities;

namespace LMS.Assessment.Api.Dtos;

public record CreateLawFirmRequest(
    string Name,
    string Address,
    string PhoneNumber,
    string Email)
{
    public LawFirm ToEntity(Guid createdBy) => new(
        Id: Guid.NewGuid(),
        Name: Name,
        Address: Address,
        PhoneNumber: PhoneNumber,
        Email: Email,
        CreatedBy: createdBy);
}
