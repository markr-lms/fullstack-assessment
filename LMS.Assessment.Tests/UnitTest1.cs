using LMS.Assessment.Api.Abstractions;
using LMS.Assessment.Api.Infrastructure;

namespace LMS.Assessment.Tests;

public class InMemoryDocumentRepositoryTests
{
    // Minimal IDocument implementation used across all tests
    private record TestDocument(string Id, string Value) : IDocument;

    private static InMemoryDocumentRepository<TestDocument> CreateRepo(
        params TestDocument[] seed)
    {
        var repo = new InMemoryDocumentRepository<TestDocument>();
        foreach (var doc in seed)
            repo.CreateAsync(doc).GetAwaiter().GetResult();
        return repo;
    }

    // ── GetByIdAsync ────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsDocument()
    {
        var doc = new TestDocument("1", "hello");
        var repo = CreateRepo(doc);

        var result = await repo.GetByIdAsync("1");

        Assert.Equal(doc, result);
    }

    [Fact]
    public async Task GetByIdAsync_MissingId_ReturnsNull()
    {
        var repo = CreateRepo();

        var result = await repo.GetByIdAsync("missing");

        Assert.Null(result);
    }

    // ── GetAllAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_EmptyStore_ReturnsEmptyPage()
    {
        var repo = CreateRepo();

        var result = await repo.GetAllAsync(pageNumber: 1, pageSize: 10);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
    }

    [Fact]
    public async Task GetAllAsync_FirstPage_ReturnsCorrectSlice()
    {
        var repo = CreateRepo(
            new TestDocument("1", "a"),
            new TestDocument("2", "b"),
            new TestDocument("3", "c"));

        var result = await repo.GetAllAsync(pageNumber: 1, pageSize: 2);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.True(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public async Task GetAllAsync_SecondPage_ReturnsRemainingItems()
    {
        var repo = CreateRepo(
            new TestDocument("1", "a"),
            new TestDocument("2", "b"),
            new TestDocument("3", "c"));

        var result = await repo.GetAllAsync(pageNumber: 2, pageSize: 2);

        Assert.Single(result.Items);
        Assert.Equal(3, result.TotalCount);
        Assert.False(result.HasNextPage);
        Assert.True(result.HasPreviousPage);
    }

    [Fact]
    public async Task GetAllAsync_PageNumberLessThanOne_ThrowsArgumentOutOfRangeException()
    {
        var repo = CreateRepo();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => repo.GetAllAsync(pageNumber: 0));
    }

    [Fact]
    public async Task GetAllAsync_PageSizeLessThanOne_ThrowsArgumentOutOfRangeException()
    {
        var repo = CreateRepo();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => repo.GetAllAsync(pageSize: 0));
    }

    // ── QueryAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task QueryAsync_MatchingPredicate_ReturnsFilteredDocuments()
    {
        var repo = CreateRepo(
            new TestDocument("1", "match"),
            new TestDocument("2", "no"),
            new TestDocument("3", "match"));

        var result = await repo.QueryAsync(d => d.Value == "match");

        Assert.Equal(2, result.Count());
        Assert.All(result, d => Assert.Equal("match", d.Value));
    }

    [Fact]
    public async Task QueryAsync_NoMatch_ReturnsEmpty()
    {
        var repo = CreateRepo(new TestDocument("1", "hello"));

        var result = await repo.QueryAsync(d => d.Value == "nope");

        Assert.Empty(result);
    }

    // ── CreateAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_NewDocument_StoresAndReturnsDocument()
    {
        var repo = CreateRepo();
        var doc = new TestDocument("1", "new");

        var result = await repo.CreateAsync(doc);

        Assert.Equal(doc, result);
        Assert.Equal(doc, await repo.GetByIdAsync("1"));
    }

    [Fact]
    public async Task CreateAsync_DuplicateId_ThrowsInvalidOperationException()
    {
        var repo = CreateRepo(new TestDocument("1", "original"));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => repo.CreateAsync(new TestDocument("1", "duplicate")));
    }

    // ── UpdateAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ExistingDocument_UpdatesAndReturnsDocument()
    {
        var repo = CreateRepo(new TestDocument("1", "old"));
        var updated = new TestDocument("1", "new");

        var result = await repo.UpdateAsync(updated);

        Assert.Equal(updated, result);
        Assert.Equal("new", (await repo.GetByIdAsync("1"))!.Value);
    }

    [Fact]
    public async Task UpdateAsync_MissingDocument_ThrowsKeyNotFoundException()
    {
        var repo = CreateRepo();

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => repo.UpdateAsync(new TestDocument("ghost", "value")));
    }

    // ── DeleteAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ExistingId_RemovesDocument()
    {
        var repo = CreateRepo(new TestDocument("1", "bye"));

        await repo.DeleteAsync("1");

        Assert.Null(await repo.GetByIdAsync("1"));
    }

    [Fact]
    public async Task DeleteAsync_MissingId_ThrowsKeyNotFoundException()
    {
        var repo = CreateRepo();

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => repo.DeleteAsync("ghost"));
    }
}

