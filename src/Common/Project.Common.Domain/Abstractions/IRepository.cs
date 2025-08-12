namespace Project.Common.Domain.Abstractions;

public interface IRepository<T> where T : class
{
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity, CancellationToken cancellationToken = default);
    void Delete(T entity, CancellationToken cancellationToken = default);
}

