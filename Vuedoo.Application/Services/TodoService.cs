using Vuedoo.Domain.Entities;
using Vuedoo.Domain.Repositories;

namespace Vuedoo.Application.Services;

public class TodoService
{
    private readonly ITodoRepository _todoRepository;

    public TodoService(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<IEnumerable<Todo>> GetAllTodosAsync(Guid userId)
    {
        return await _todoRepository.GetAllAsync(userId);
    }

    public async Task<Todo?> GetTodoByIdAsync(Guid id, Guid userId)
    {
        return await _todoRepository.GetByIdAsync(id, userId);
    }

    public async Task<Todo> CreateTodoAsync(string text, Guid userId)
    {
        var todo = new Todo
        {
            Text = text,
            UserId = userId,
            IsComplete = false
        };

        return await _todoRepository.CreateAsync(todo);
    }

    public async Task<Todo?> UpdateTodoAsync(Guid id, string? text, bool? isComplete, Guid userId)
    {
        var existingTodo = await _todoRepository.GetByIdAsync(id, userId);
        if (existingTodo == null)
        {
            return null;
        }

        if (text != null)
        {
            existingTodo.Text = text;
        }

        if (isComplete.HasValue)
        {
            existingTodo.IsComplete = isComplete.Value;
        }

        existingTodo.UpdatedDate = DateTime.UtcNow;

        return await _todoRepository.UpdateAsync(existingTodo);
    }

    public async Task<bool> DeleteTodoAsync(Guid id, Guid userId)
    {
        return await _todoRepository.DeleteAsync(id, userId);
    }
}
