using LMS.Assessment.Api.Abstractions;
using LMS.Assessment.Api.Controllers;
using LMS.Assessment.Api.Entities;
using LMS.Assessment.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Assessment.Tests;

public class DocumentsControllerTests
{
    private static Document MakeDocument(Guid? id = null) => new(
        id ?? Guid.NewGuid(),
        "Contract Agreement",
        "PDF",
        Guid.NewGuid(),
        Guid.NewGuid(),
        DateTime.UtcNow,
        Guid.NewGuid());

    private static async Task<DocumentsController> CreateSut(params Document[] seed)
    {
        var repo = new InMemoryRepository<Document>();

        foreach (var document in seed)
            await repo.CreateAsync(document);

        return new DocumentsController(repo);
    }

    #region GetAll

    [Fact]
    public async Task GetAll_EmptyStore_ReturnsOkWithEmptyPage()
    {
        // Arrange
        var sut = await CreateSut();

        // Act
        var result = await sut.GetAll();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var paged = Assert.IsType<PaginatedList<Document>>(ok.Value);
        Assert.Empty(paged.Items);
        Assert.Equal(0, paged.TotalCount);
    }

    [Fact]
    public async Task GetAll_WithItems_ReturnsOkWithPagedResult()
    {
        // Arrange
        var sut = await CreateSut(MakeDocument(), MakeDocument(), MakeDocument());

        // Act
        var result = await sut.GetAll(pageNumber: 1, pageSize: 2);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var paged = Assert.IsType<PaginatedList<Document>>(ok.Value);
        Assert.Equal(2, paged.Items.Count);
        Assert.Equal(3, paged.TotalCount);
        Assert.Equal(2, paged.TotalPages);
    }

    #endregion

    #region GetById

    [Fact]
    public async Task GetById_ExistingId_ReturnsOkWithEntity()
    {
        // Arrange
        var document = MakeDocument();
        var sut = await CreateSut(document);

        // Act
        var result = await sut.GetById(document.Id);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(document, ok.Value);
    }

    [Fact]
    public async Task GetById_MissingId_ReturnsNotFound()
    {
        // Arrange
        var sut = await CreateSut();

        // Act
        var result = await sut.GetById(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region Create

    [Fact]
    public async Task Create_ValidEntity_ReturnsCreatedAtAction()
    {
        // Arrange
        var document = MakeDocument();
        var sut = await CreateSut();

        // Act
        var result = await sut.Create(document);

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(sut.GetById), created.ActionName);
        Assert.Equal(document.Id, created.RouteValues!["id"]);
        Assert.Equal(document, created.Value);
    }

    #endregion

    #region Update

    [Fact]
    public async Task Update_IdMismatch_ReturnsBadRequest()
    {
        // Arrange
        var document = MakeDocument();
        var sut = await CreateSut(document);

        // Act
        var result = await sut.Update(Guid.NewGuid(), document);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_ExistingEntity_ReturnsOkWithUpdatedEntity()
    {
        // Arrange
        var id = Guid.NewGuid();
        var original = MakeDocument(id);
        var sut = await CreateSut(original);
        var updated = original with { Title = "Updated Contract" };

        // Act
        var result = await sut.Update(id, updated);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(updated, ok.Value);
    }

    [Fact]
    public async Task Update_MissingEntity_ReturnsNotFound()
    {
        // Arrange
        var document = MakeDocument();
        var sut = await CreateSut();

        // Act
        var result = await sut.Update(document.Id, document);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion
}
