using LMS.Assessment.Api.Abstractions;
using LMS.Assessment.Api.Controllers;
using LMS.Assessment.Api.Entities;
using LMS.Assessment.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Assessment.Tests;

public class LawFirmsControllerTests
{
    private static LawFirm MakeLawFirm(Guid? id = null) => new(
        id ?? Guid.NewGuid(),
        "Acme Law",
        "123 Main St",
        "555-1234",
        "acme@law.com",
        Guid.NewGuid());

    private static async Task<LawFirmsController> CreateSut(params LawFirm[] seed)
    {
        var repo = new InMemoryRepository<LawFirm>();

        foreach (var firm in seed)
            await repo.CreateAsync(firm);

        return new LawFirmsController(repo);
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
        var paged = Assert.IsType<PagedResult<LawFirm>>(ok.Value);
        Assert.Empty(paged.Items);
        Assert.Equal(0, paged.TotalCount);
    }

    [Fact]
    public async Task GetAll_WithItems_ReturnsOkWithPagedResult()
    {
        // Arrange
        var sut = await CreateSut(MakeLawFirm(), MakeLawFirm(), MakeLawFirm());

        // Act
        var result = await sut.GetAll(pageNumber: 1, pageSize: 2);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var paged = Assert.IsType<PagedResult<LawFirm>>(ok.Value);
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
        var firm = MakeLawFirm();
        var sut = await CreateSut(firm);

        // Act
        var result = await sut.GetById(firm.Id);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(firm, ok.Value);
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
        var firm = MakeLawFirm();
        var sut = await CreateSut();

        // Act
        var result = await sut.Create(firm);

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(sut.GetById), created.ActionName);
        Assert.Equal(firm.Id, created.RouteValues!["id"]);
        Assert.Equal(firm, created.Value);
    }

    #endregion

    #region Update

    [Fact]
    public async Task Update_IdMismatch_ReturnsBadRequest()
    {
        // Arrange
        var firm = MakeLawFirm();
        var sut = await CreateSut(firm);

        // Act
        var result = await sut.Update(Guid.NewGuid(), firm);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_ExistingEntity_ReturnsOkWithUpdatedEntity()
    {
        // Arrange
        var id = Guid.NewGuid();
        var original = MakeLawFirm(id);
        var sut = await CreateSut(original);
        var updated = original with { Name = "Updated Law" };

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
        var firm = MakeLawFirm();
        var sut = await CreateSut();

        // Act
        var result = await sut.Update(firm.Id, firm);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion
}
