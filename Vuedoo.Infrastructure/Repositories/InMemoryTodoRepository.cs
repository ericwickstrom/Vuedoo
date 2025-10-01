using Vuedoo.Domain.Entities;
using Vuedoo.Domain.Repositories;

namespace Vuedoo.Infrastructure.Repositories;

public class InMemoryTodoRepository : ITodoRepository
{
    private readonly List<Todo> _todos = new();

    public InMemoryTodoRepository()
    {
        SeedData();
    }

    private void SeedData()
    {
        var tempUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var baseDate = DateTime.UtcNow;

        _todos.AddRange(new[]
        {
            new Todo
            {
                Id = Guid.NewGuid(),
                Text = "Learn Vue 3 Composition API",
                IsComplete = true,
                CreatedDate = baseDate.AddDays(-5),
                UpdatedDate = baseDate.AddDays(-2),
                UserId = tempUserId
            },
            new Todo
            {
                Id = Guid.NewGuid(),
                Text = "Build RESTful API with .NET",
                IsComplete = true,
                CreatedDate = baseDate.AddDays(-4),
                UpdatedDate = baseDate.AddDays(-1),
                UserId = tempUserId
            },
            new Todo
            {
                Id = Guid.NewGuid(),
                Text = "Connect frontend to backend",
                IsComplete = false,
                CreatedDate = baseDate.AddDays(-3),
                UserId = tempUserId
            },
            new Todo
            {
                Id = Guid.NewGuid(),
                Text = "Add user authentication",
                IsComplete = false,
                CreatedDate = baseDate.AddDays(-2),
                UserId = tempUserId
            },
            new Todo
            {
                Id = Guid.NewGuid(),
                Text = "Deploy to production",
                IsComplete = false,
                CreatedDate = baseDate.AddDays(-1),
                UserId = tempUserId
            }
        });
    }

    public Task<IEnumerable<Todo>> GetAllAsync(Guid userId)
    {
        var todos = _todos.Where(t => t.UserId == userId).ToList();
        return Task.FromResult<IEnumerable<Todo>>(todos);
    }

    public Task<Todo?> GetByIdAsync(Guid id, Guid userId)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id && t.UserId == userId);
        return Task.FromResult(todo);
    }

    public Task<Todo> CreateAsync(Todo todo)
    {
        _todos.Add(todo);
        return Task.FromResult(todo);
    }

    public Task<Todo?> UpdateAsync(Todo todo)
    {
        var existingTodo = _todos.FirstOrDefault(t => t.Id == todo.Id && t.UserId == todo.UserId);
        if (existingTodo == null)
        {
            return Task.FromResult<Todo?>(null);
        }

        existingTodo.Text = todo.Text;
        existingTodo.IsComplete = todo.IsComplete;
        existingTodo.UpdatedDate = todo.UpdatedDate;

        return Task.FromResult<Todo?>(existingTodo);
    }

    public Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id && t.UserId == userId);
        if (todo == null)
        {
            return Task.FromResult(false);
        }

        _todos.Remove(todo);
        return Task.FromResult(true);
    }
}
