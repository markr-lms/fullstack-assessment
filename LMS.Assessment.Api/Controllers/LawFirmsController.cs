using LMS.Assessment.Api.Abstractions;
using LMS.Assessment.Api.Dtos;
using LMS.Assessment.Api.Entities;
using LMS.Assessment.Api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Assessment.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class LawFirmsController : ControllerBase
{
    private readonly IRepository<LawFirm> _repository;

    public LawFirmsController(IRepository<LawFirm> repository)
    {
        _repository = repository;
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateLawFirmRequest lawFirm)
    {
        if (id != lawFirm.Id)
            return BadRequest("Id in the URL does not match the Id in the body.");

        if (Request.GetUserId() is not Guid userId)
            return Unauthorized("User ID is missing from the request.");

        var entity = lawFirm.ToEntity(userId);

        try
        {
            var updated = await _repository.UpdateAsync(entity);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
