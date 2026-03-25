namespace LMS.Assessment.Api.Abstractions;

public interface IRepository<T> where T : IEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<PagedResult<T>> GetAllAsync(int pageNumber = 1, int pageSize = 20);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}
