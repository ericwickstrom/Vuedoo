using Vuedoo.Domain.Entities;

namespace Vuedoo.Domain.Repositories;

public interface ITodoRepository
{
    Task<IEnumerable<Todo>> GetAllAsync(Guid userId);
    Task<Todo?> GetByIdAsync(Guid id, Guid userId);
    Task<Todo> CreateAsync(Todo todo);
    Task<Todo?> UpdateAsync(Todo todo);
    Task<bool> DeleteAsync(Guid id, Guid userId);
}
