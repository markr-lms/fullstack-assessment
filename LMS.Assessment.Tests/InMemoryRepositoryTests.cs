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

    // ── GetByIdAsync ────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsEntity()
    {
        var id = Guid.NewGuid();
        var entity = new TestEntity(id, "hello");
        var repo = await CreateRepo(entity);

        var result = await repo.GetByIdAsync(id);

        Assert.Equal(entity, result);
    }

    [Fact]
    public async Task GetByIdAsync_MissingId_ReturnsNull()
    {
        var repo = await CreateRepo();

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    // ── GetAllAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_EmptyStore_ReturnsEmptyPage()
    {
        var repo = await CreateRepo();

        var result = await repo.GetAllAsync(pageNumber: 1, pageSize: 10);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
    }

    [Fact]
    public async Task GetAllAsync_FirstPage_ReturnsCorrectSlice()
    {
        var repo = await CreateRepo(
            new TestEntity(Guid.NewGuid(), "a"),
            new TestEntity(Guid.NewGuid(), "b"),
            new TestEntity(Guid.NewGuid(), "c"));

        var result = await repo.GetAllAsync(pageNumber: 1, pageSize: 2);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task GetAllAsync_SecondPage_ReturnsRemainingItems()
    {
        var repo = await CreateRepo(
            new TestEntity(Guid.NewGuid(), "a"),
            new TestEntity(Guid.NewGuid(), "b"),
            new TestEntity(Guid.NewGuid(), "c"));

        var result = await repo.GetAllAsync(pageNumber: 2, pageSize: 2);

        Assert.Single(result.Items);
        Assert.Equal(3, result.TotalCount);
    }

    [Fact]
    public async Task GetAllAsync_PageNumberLessThanOne_ThrowsArgumentOutOfRangeException()
    {
        var repo = await CreateRepo();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => repo.GetAllAsync(pageNumber: 0));
    }

    [Fact]
    public async Task GetAllAsync_PageSizeLessThanOne_ThrowsArgumentOutOfRangeException()
    {
        var repo = await CreateRepo();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => repo.GetAllAsync(pageSize: 0));
    }

    // ── CreateAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_NewEntity_StoresAndReturnsEntity()
    {
        var id = Guid.NewGuid();
        var repo = await CreateRepo();
        var entity = new TestEntity(id, "new");

        var result = await repo.CreateAsync(entity);

        Assert.Equal(entity, result);
        Assert.Equal(entity, await repo.GetByIdAsync(id));
    }

    [Fact]
    public async Task CreateAsync_DuplicateId_ThrowsInvalidOperationException()
    {
        var id = Guid.NewGuid();
        var repo = await CreateRepo(new TestEntity(id, "original"));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => repo.CreateAsync(new TestEntity(id, "duplicate")));
    }

    // ── UpdateAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ExistingEntity_UpdatesAndReturnsEntity()
    {
        var id = Guid.NewGuid();
        var repo = await CreateRepo(new TestEntity(id, "old"));
        var updated = new TestEntity(id, "new");

        var result = await repo.UpdateAsync(updated);

        Assert.Equal(updated, result);
        Assert.Equal("new", (await repo.GetByIdAsync(id))!.Value);
    }

    [Fact]
    public async Task UpdateAsync_MissingEntity_ThrowsKeyNotFoundException()
    {
        var repo = await CreateRepo();

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => repo.UpdateAsync(new TestEntity(Guid.NewGuid(), "value")));
    }

    // ── DeleteAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ExistingId_RemovesEntity()
    {
        var id = Guid.NewGuid();
        var repo = await CreateRepo(new TestEntity(id, "bye"));

        await repo.DeleteAsync(id);

        Assert.Null(await repo.GetByIdAsync(id));
    }

    [Fact]
    public async Task DeleteAsync_MissingId_ThrowsKeyNotFoundException()
    {
        var repo = await CreateRepo();

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => repo.DeleteAsync(Guid.NewGuid()));
    }
}

