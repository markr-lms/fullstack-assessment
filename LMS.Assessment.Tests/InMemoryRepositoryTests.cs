using LMS.Assessment.Api.Abstractions;
using LMS.Assessment.Api.Infrastructure;

namespace LMS.Assessment.Tests;

public class InMemoryRepositoryTests
{
    // Minimal implementation used across all tests
    private record TestEntity(string Id, string Value) : IEntity;

    private static async Task<InMemoryRepository<TestEntity>> CreateRepo(
        params TestEntity[] seed)
    {
        var repo = new InMemoryRepository<TestEntity>();

        foreach (var doc in seed)
            await repo.CreateAsync(doc);

        return repo;
    }

    // ── GetByIdAsync ────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsDocument()
    {
        var doc = new TestEntity("1", "hello");
        var repo = await CreateRepo(doc);

        var result = await repo.GetByIdAsync("1");

        Assert.Equal(doc, result);
    }

    [Fact]
    public async Task GetByIdAsync_MissingId_ReturnsNull()
    {
        var repo = await CreateRepo();

        var result = await repo.GetByIdAsync("missing");

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
            new TestEntity("1", "a"),
            new TestEntity("2", "b"),
            new TestEntity("3", "c"));

        var result = await repo.GetAllAsync(pageNumber: 1, pageSize: 2);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task GetAllAsync_SecondPage_ReturnsRemainingItems()
    {
        var repo = await CreateRepo(
            new TestEntity("1", "a"),
            new TestEntity("2", "b"),
            new TestEntity("3", "c"));

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
    public async Task CreateAsync_NewDocument_StoresAndReturnsDocument()
    {
        var repo = await CreateRepo();
        var doc = new TestEntity("1", "new");

        var result = await repo.CreateAsync(doc);

        Assert.Equal(doc, result);
        Assert.Equal(doc, await repo.GetByIdAsync("1"));
    }

    [Fact]
    public async Task CreateAsync_DuplicateId_ThrowsInvalidOperationException()
    {
        var repo = await CreateRepo(new TestEntity("1", "original"));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => repo.CreateAsync(new TestEntity("1", "duplicate")));
    }

    // ── UpdateAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ExistingDocument_UpdatesAndReturnsDocument()
    {
        var repo = await CreateRepo(new TestEntity("1", "old"));
        var updated = new TestEntity("1", "new");

        var result = await repo.UpdateAsync(updated);

        Assert.Equal(updated, result);
        Assert.Equal("new", (await repo.GetByIdAsync("1"))!.Value);
    }

    [Fact]
    public async Task UpdateAsync_MissingDocument_ThrowsKeyNotFoundException()
    {
        var repo = await CreateRepo();

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => repo.UpdateAsync(new TestEntity("ghost", "value")));
    }

    // ── DeleteAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ExistingId_RemovesDocument()
    {
        var repo = await CreateRepo(new TestEntity("1", "bye"));

        await repo.DeleteAsync("1");

        Assert.Null(await repo.GetByIdAsync("1"));
    }

    [Fact]
    public async Task DeleteAsync_MissingId_ThrowsKeyNotFoundException()
    {
        var repo = await CreateRepo();

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => repo.DeleteAsync("ghost"));
    }
}

