using LMS.Assessment.Api.Abstractions;
using LMS.Assessment.Api.Infrastructure;

namespace LMS.Assessment.Tests;

public class InMemoryRepositoryTests
{
    // Minimal implementation used across all tests
    private record TestEntity(Guid Id, string Value) : IEntity
    {
        public Guid CreatedBy { get; init; } = Guid.NewGuid();
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }

    private static async Task<InMemoryRepository<TestEntity>> CreateRepo(params TestEntity[] seed)
    {
        var repo = new InMemoryRepository<TestEntity>();

        foreach (var entity in seed)
            await repo.CreateAsync(entity);

        return repo;
    }

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsEntity()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new TestEntity(id, "hello");
        var repo = await CreateRepo(entity);

        // Act
        var result = await repo.GetByIdAsync(id);

        // Assert
        Assert.Equal(entity, result);
    }

    [Fact]
    public async Task GetByIdAsync_MissingId_ReturnsNull()
    {
        // Arrange
        var repo = await CreateRepo();

        // Act
        var result = await repo.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_EmptyStore_ReturnsEmptyPage()
    {
        // Arrange
        var repo = await CreateRepo();

        // Act
        var result = await repo.GetAllAsync(pageNumber: 1, pageSize: 10);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
    }

    [Fact]
    public async Task GetAllAsync_FirstPage_ReturnsCorrectSlice()
    {
        // Arrange
        var repo = await CreateRepo(
            new TestEntity(Guid.NewGuid(), "a"),
            new TestEntity(Guid.NewGuid(), "b"),
            new TestEntity(Guid.NewGuid(), "c"));

        // Act
        var result = await repo.GetAllAsync(pageNumber: 1, pageSize: 2);

        // Assert
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task GetAllAsync_SecondPage_ReturnsRemainingItems()
    {
        // Arrange
        var repo = await CreateRepo(
            new TestEntity(Guid.NewGuid(), "a"),
            new TestEntity(Guid.NewGuid(), "b"),
            new TestEntity(Guid.NewGuid(), "c"));

        // Act
        var result = await repo.GetAllAsync(pageNumber: 2, pageSize: 2);

        // Assert
        Assert.Single(result.Items);
        Assert.Equal(3, result.TotalCount);
    }

    [Fact]
    public async Task GetAllAsync_PageNumberLessThanOne_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var repo = await CreateRepo();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => repo.GetAllAsync(pageNumber: 0));
    }

    [Fact]
    public async Task GetAllAsync_PageSizeLessThanOne_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var repo = await CreateRepo();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => repo.GetAllAsync(pageSize: 0));
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_NewEntity_StoresAndReturnsEntity()
    {
        // Arrange
        var id = Guid.NewGuid();
        var repo = await CreateRepo();
        var entity = new TestEntity(id, "new");

        // Act
        var result = await repo.CreateAsync(entity);

        // Assert
        Assert.Equal(entity, result);
        Assert.Equal(entity, await repo.GetByIdAsync(id));
    }

    [Fact]
    public async Task CreateAsync_DuplicateId_ThrowsInvalidOperationException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var repo = await CreateRepo(new TestEntity(id, "original"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => repo.CreateAsync(new TestEntity(id, "duplicate")));
    }

    #endregion
}

