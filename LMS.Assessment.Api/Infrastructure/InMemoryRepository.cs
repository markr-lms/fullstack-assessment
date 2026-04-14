using System.Collections.Concurrent;
using LMS.Assessment.Api.Abstractions;

namespace LMS.Assessment.Api.Infrastructure;

public class InMemoryRepository<T> where T : IEntity
{
    private readonly ConcurrentDictionary<Guid, T> _store = new();

    public async Task<T?> GetByIdAsync(Guid id)
    {
        _store.TryGetValue(id, out var entity);

        await SimulateDbOperation();
        return entity;
    }

    public async Task<PaginatedList<T>> GetAllAsync(int pageNumber = 1, int pageSize = 20)
    {
        if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be at least 1.");
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be at least 1.");

        var all = _store.Values.ToList();

        var items = all
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        await SimulateDbOperation();
        return new PaginatedList<T>(items, all.Count, pageNumber, pageSize);
    }

    public async Task<T> CreateAsync(T entity)
    {
        if (!_store.TryAdd(entity.Id, entity))
            throw new InvalidOperationException($"An entity with id '{entity.Id}' already exists.");

        await SimulateDbOperation();
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        if (!_store.ContainsKey(entity.Id))
            throw new KeyNotFoundException($"No entity with id '{entity.Id}' was found.");

        _store[entity.Id] = entity;

        await SimulateDbOperation();
        return entity;
    }

    private static async Task SimulateDbOperation()
    {
        // Simulate some latency to mimic a real database operation
        await Task.Delay(new Random().Next(40, 80));
    }
}
