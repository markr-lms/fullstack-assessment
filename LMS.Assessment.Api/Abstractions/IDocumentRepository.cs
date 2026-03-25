using System.Linq.Expressions;

namespace LMS.Assessment.Api.Abstractions;

public interface IDocumentRepository<T> where T : IDocument
{
    Task<T?> GetByIdAsync(string id);
    Task<PagedResult<T>> GetAllAsync(int pageNumber = 1, int pageSize = 20);
    Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> predicate);
    Task<T> CreateAsync(T document);
    Task<T> UpdateAsync(T document);
    Task DeleteAsync(string id);
}
