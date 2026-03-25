using System.Collections.Concurrent;
using System.Linq.Expressions;
using LMS.Assessment.Api.Abstractions;

namespace LMS.Assessment.Api.Infrastructure;

public class InMemoryDocumentRepository<T> : IDocumentRepository<T> where T : IDocument
{
    private readonly ConcurrentDictionary<string, T> _store = new();

    public Task<T?> GetByIdAsync(string id)
    {
        _store.TryGetValue(id, out var document);
        return Task.FromResult(document);
    }

    public Task<PagedResult<T>> GetAllAsync(int pageNumber = 1, int pageSize = 20)
    {
        if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be at least 1.");
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be at least 1.");

        var all = _store.Values.ToList();
        var items = all.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PagedResult<T>(items, all.Count, pageNumber, pageSize));
    }

    public Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> predicate)
    {
        var filter = predicate.Compile();
        var results = _store.Values.Where(filter).ToList();
        return Task.FromResult<IEnumerable<T>>(results);
    }

    public Task<T> CreateAsync(T document)
    {
        if (!_store.TryAdd(document.Id, document))
            throw new InvalidOperationException($"A document with id '{document.Id}' already exists.");

        return Task.FromResult(document);
    }

    public Task<T> UpdateAsync(T document)
    {
        if (!_store.ContainsKey(document.Id))
            throw new KeyNotFoundException($"No document with id '{document.Id}' was found.");

        _store[document.Id] = document;
        return Task.FromResult(document);
    }

    public Task DeleteAsync(string id)
    {
        if (!_store.TryRemove(id, out _))
            throw new KeyNotFoundException($"No document with id '{id}' was found.");

        return Task.CompletedTask;
    }
}
