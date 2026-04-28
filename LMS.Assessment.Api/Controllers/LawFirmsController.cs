using Bogus;
using LMS.Assessment.Api.Dtos;
using LMS.Assessment.Api.Entities;
using LMS.Assessment.Api.Helpers;
using LMS.Assessment.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Assessment.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class LawFirmsController : ControllerBase
{
    private readonly InMemoryRepository<LawFirm> _repository;

    public LawFirmsController(LawFirm[]? seedLawFirmData = null)
    {
        _repository = new InMemoryRepository<LawFirm>();
        var lawFirmsToSeed = seedLawFirmData ?? GenerateFakeLawFirms();

        foreach (var lawFirm in lawFirmsToSeed)
        {
            _repository.CreateAsync(lawFirm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _repository.GetAllAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var lawFirm = await _repository.GetByIdAsync(id);
        return lawFirm is null ? NotFound() : Ok(lawFirm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateLawFirmRequest lawFirm)
    {
        if (Request.GetUserId() is not Guid userId)
            return Unauthorized("User ID is missing from the request.");

        var entity = lawFirm.ToEntity(userId);
        var created = await _repository.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    private static LawFirm[] GenerateFakeLawFirms()
    {
        var lawFirmFaker = new Faker<LawFirm>("en_GB")
            .CustomInstantiator(f => new LawFirm(
                Guid.NewGuid(),
                f.Company.CompanyName(),
                f.Address.FullAddress(),
                f.Phone.PhoneNumber(),
                f.Internet.Email(),
                Guid.NewGuid())
            {
                CreatedAt = f.Date.Past(1)
            });

        return [.. lawFirmFaker.Generate(50)];
    }
}
