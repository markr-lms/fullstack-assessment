using System.Collections.Concurrent;
using System.Linq.Expressions;
using LMS.Assessment.Api.Abstractions;

namespace LMS.Assessment.Api.Infrastructure;

public class InMemoryDocumentRepository<T> : IDocumentRepository<T> where T : IDocument
{
    private readonly ConcurrentDictionary<string, T> _store = new();

    public Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        _store.TryGetValue(id, out var document);
        return Task.FromResult(document);
    }

    public Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<T>>(_store.Values.ToList());
    }

    public Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var filter = predicate.Compile();
        var results = _store.Values.Where(filter).ToList();
        return Task.FromResult<IEnumerable<T>>(results);
    }

    public Task<T> CreateAsync(T document, CancellationToken cancellationToken = default)
    {
        if (!_store.TryAdd(document.Id, document))
            throw new InvalidOperationException($"A document with id '{document.Id}' already exists.");

        return Task.FromResult(document);
    }

    public Task<T> UpdateAsync(T document, CancellationToken cancellationToken = default)
    {
        if (!_store.ContainsKey(document.Id))
            throw new KeyNotFoundException($"No document with id '{document.Id}' was found.");

        _store[document.Id] = document;
        return Task.FromResult(document);
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        if (!_store.TryRemove(id, out _))
            throw new KeyNotFoundException($"No document with id '{id}' was found.");

        return Task.CompletedTask;
    }
}
