using System.Linq.Expressions;

namespace LMS.Assessment.Api.Abstractions;

public interface IDocumentRepository<T> where T : IDocument
{
    Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T> CreateAsync(T document, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T document, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
