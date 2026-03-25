using LMS.Assessment.Api.Abstractions;
using LMS.Assessment.Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Assessment.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class LawFirmsController : ControllerBase
{
    private readonly IDocumentRepository<LawFirm> _repository;

    public LawFirmsController(IDocumentRepository<LawFirm> repository)
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
    public async Task<IActionResult> GetById(string id)
    {
        var lawFirm = await _repository.GetByIdAsync(id);
        return lawFirm is null ? NotFound() : Ok(lawFirm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(LawFirm lawFirm)
    {
        var created = await _repository.CreateAsync(lawFirm);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, LawFirm lawFirm)
    {
        if (id != lawFirm.Id)
            return BadRequest("Id in the URL does not match the Id in the body.");

        try
        {
            var updated = await _repository.UpdateAsync(lawFirm);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
