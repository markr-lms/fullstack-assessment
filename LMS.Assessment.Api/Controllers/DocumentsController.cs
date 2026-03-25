using LMS.Assessment.Api.Abstractions;
using LMS.Assessment.Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Assessment.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IRepository<Document> _repository;

    public DocumentsController(IRepository<Document> repository)
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
        var document = await _repository.GetByIdAsync(id);
        return document is null ? NotFound() : Ok(document);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Document document)
    {
        var created = await _repository.CreateAsync(document);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Document document)
    {
        if (id != document.Id)
            return BadRequest("Id in the URL does not match the Id in the body.");

        try
        {
            var updated = await _repository.UpdateAsync(document);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
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
