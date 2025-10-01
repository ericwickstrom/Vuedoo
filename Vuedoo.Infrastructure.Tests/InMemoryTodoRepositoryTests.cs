using FluentAssertions;
using Vuedoo.Domain.Entities;
using Vuedoo.Infrastructure.Repositories;

namespace Vuedoo.Infrastructure.Tests;

public class InMemoryTodoRepositoryTests
{
    private readonly Guid _tempUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly Guid _otherUserId = Guid.Parse("00000000-0000-0000-0000-000000000002");

    [Fact]
    public async Task Constructor_ShouldSeedDataForTempUser()
    {
        // Arrange & Act
        var repository = new InMemoryTodoRepository();
        var todos = await repository.GetAllAsync(_tempUserId);

        // Assert
        todos.Should().HaveCount(5);
        todos.Should().Contain(t => t.Text == "Learn Vue 3 Composition API" && t.IsComplete == true);
        todos.Should().Contain(t => t.Text == "Build RESTful API with .NET" && t.IsComplete == true);
        todos.Should().Contain(t => t.Text == "Connect frontend to backend" && t.IsComplete == false);
        todos.Should().Contain(t => t.Text == "Add user authentication" && t.IsComplete == false);
        todos.Should().Contain(t => t.Text == "Deploy to production" && t.IsComplete == false);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyUserTodos()
    {
        // Arrange
        var repository = new InMemoryTodoRepository();
        var newTodo = new Todo
        {
            Text = "Other user todo",
            UserId = _otherUserId
        };
        await repository.CreateAsync(newTodo);

        // Act
        var userTodos = await repository.GetAllAsync(_tempUserId);
        var otherTodos = await repository.GetAllAsync(_otherUserId);

        // Assert
        userTodos.Should().HaveCount(5);
        userTodos.Should().AllSatisfy(t => t.UserId.Should().Be(_tempUserId));
        otherTodos.Should().HaveCount(1);
        otherTodos.Should().AllSatisfy(t => t.UserId.Should().Be(_otherUserId));
    }

    [Fact]
    public async Task GetAllAsync_WhenNoTodos_ShouldReturnEmptyList()
    {
        // Arrange
        var repository = new InMemoryTodoRepository();

        // Act
        var todos = await repository.GetAllAsync(_otherUserId);

        // Assert
        todos.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WhenTodoExistsForUser_ShouldReturnTodo()
    {
        // Arrange
        var repository = new InMemoryTodoRepository();
        var allTodos = await repository.GetAllAsync(_tempUserId);
        var targetTodo = allTodos.First();

        // Act
        var result = await repository.GetByIdAsync(targetTodo.Id, _tempUserId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(targetTodo.Id);
        result.Text.Should().Be(targetTodo.Text);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTodoDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var repository = new InMemoryTodoRepository();
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.GetByIdAsync(nonExistentId, _tempUserId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenTodoExistsButForDifferentUser_ShouldReturnNull()
    {
        // Arrange
        var repository = new InMemoryTodoRepository();
        var allTodos = await repository.GetAllAsync(_tempUserId);
        var targetTodo = allTodos.First();

        // Act
        var result = await repository.GetByIdAsync(targetTodo.Id, _otherUserId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldAddTodoToRepository()
    {
        // Arrange
        var repository = new InMemoryTodoRepository();
        var newTodo = new Todo
        {
            Text = "New test todo",
            UserId = _tempUserId,
            IsComplete = false
        };

        // Act
        var result = await repository.CreateAsync(newTodo);
        var allTodos = await repository.GetAllAsync(_tempUserId);

        // Assert
        result.Should().Be(newTodo);
        allTodos.Should().Contain(t => t.Id == newTodo.Id);
        allTodos.Should().HaveCount(6); // 5 seeded + 1 new
    }

    [Fact]
    public async Task UpdateAsync_WhenTodoExists_ShouldUpdateTodo()
    {
        // Arrange
        var repository = new InMemoryTodoRepository();
        var allTodos = await repository.GetAllAsync(_tempUserId);
        var todoToUpdate = allTodos.First();
        todoToUpdate.Text = "Updated text";
        todoToUpdate.IsComplete = true;
        todoToUpdate.UpdatedDate = DateTime.UtcNow;

        // Act
        var result = await repository.UpdateAsync(todoToUpdate);

        // Assert
        result.Should().NotBeNull();
        result!.Text.Should().Be("Updated text");
        result.IsComplete.Should().BeTrue();
        result.UpdatedDate.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_WhenTodoDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var repository = new InMemoryTodoRepository();
        var nonExistentTodo = new Todo
        {
            Id = Guid.NewGuid(),
            Text = "Non-existent",
            UserId = _tempUserId
        };

        // Act
        var result = await repository.UpdateAsync(nonExistentTodo);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WhenTodoExistsButForDifferentUser_ShouldReturnNull()
    {
        // Arrange
        var repository = new InMemoryTodoRepository();
        var allTodos = await repository.GetAllAsync(_tempUserId);
        var existingTodo = allTodos.First();

        // Create a new todo object with the same ID but different user
        var todoWithDifferentUser = new Todo
        {
            Id = existingTodo.Id,
            Text = "Updated text",
            IsComplete = true,
            UserId = _otherUserId // Different user trying to update
        };

        // Act
        var result = await repository.UpdateAsync(todoWithDifferentUser);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenTodoExists_ShouldRemoveTodoAndReturnTrue()
    {
        // Arrange
        var repository = new InMemoryTodoRepository();
        var allTodos = await repository.GetAllAsync(_tempUserId);
        var todoToDelete = allTodos.First();

        // Act
        var result = await repository.DeleteAsync(todoToDelete.Id, _tempUserId);
        var remainingTodos = await repository.GetAllAsync(_tempUserId);

        // Assert
        result.Should().BeTrue();
        remainingTodos.Should().HaveCount(4);
        remainingTodos.Should().NotContain(t => t.Id == todoToDelete.Id);
    }

    [Fact]
    public async Task DeleteAsync_WhenTodoDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var repository = new InMemoryTodoRepository();
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.DeleteAsync(nonExistentId, _tempUserId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WhenTodoExistsButForDifferentUser_ShouldReturnFalse()
    {
        // Arrange
        var repository = new InMemoryTodoRepository();
        var allTodos = await repository.GetAllAsync(_tempUserId);
        var todoToDelete = allTodos.First();

        // Act
        var result = await repository.DeleteAsync(todoToDelete.Id, _otherUserId);
        var remainingTodos = await repository.GetAllAsync(_tempUserId);

        // Assert
        result.Should().BeFalse();
        remainingTodos.Should().HaveCount(5); // Nothing deleted
    }

    [Fact]
    public async Task UserIsolation_UsersShouldNotSeeEachOthersTodos()
    {
        // Arrange
        var repository = new InMemoryTodoRepository();
        var user1Todo = new Todo { Text = "User 1 todo", UserId = _tempUserId };
        var user2Todo = new Todo { Text = "User 2 todo", UserId = _otherUserId };
        await repository.CreateAsync(user1Todo);
        await repository.CreateAsync(user2Todo);

        // Act
        var user1Todos = await repository.GetAllAsync(_tempUserId);
        var user2Todos = await repository.GetAllAsync(_otherUserId);

        // Assert
        user1Todos.Should().NotContain(t => t.Id == user2Todo.Id);
        user2Todos.Should().NotContain(t => t.Id == user1Todo.Id);
        user1Todos.Should().Contain(t => t.Id == user1Todo.Id);
        user2Todos.Should().Contain(t => t.Id == user2Todo.Id);
    }
}
