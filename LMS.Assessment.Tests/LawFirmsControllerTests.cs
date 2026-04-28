using LMS.Assessment.Api.Abstractions;
using LMS.Assessment.Api.Controllers;
using LMS.Assessment.Api.Dtos;
using LMS.Assessment.Api.Entities;
using Microsoft.AspNetCore.Http;
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

    private static async Task<LawFirmsController> CreateSut(params LawFirm[] seedLawFirmData)
    {
        var controller = new LawFirmsController(seedLawFirmData)
        {
            ControllerContext = new ControllerContext()
        };

        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        controller.ControllerContext.HttpContext.Request.Headers.Append("X-User-Id", Guid.NewGuid().ToString());

        return controller;
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
        var paged = Assert.IsType<PaginatedList<LawFirm>>(ok.Value);
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
        var paged = Assert.IsType<PaginatedList<LawFirm>>(ok.Value);
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
        var request = new CreateLawFirmRequest("Acme Law", "123 Main St", "555-1234", "acme@law.com");
        var sut = await CreateSut();

        // Act
        var result = await sut.Create(request);

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(sut.GetById), created.ActionName);
        var createdFirm = Assert.IsType<LawFirm>(created.Value);
        Assert.Equal(createdFirm.Id, created.RouteValues!["id"]);
    }

    #endregion
}
