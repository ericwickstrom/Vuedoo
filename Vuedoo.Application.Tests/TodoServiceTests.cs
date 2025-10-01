using FluentAssertions;
using Moq;
using Vuedoo.Application.Services;
using Vuedoo.Domain.Entities;
using Vuedoo.Domain.Repositories;

namespace Vuedoo.Application.Tests;

public class TodoServiceTests
{
    private readonly Mock<ITodoRepository> _mockRepository;
    private readonly TodoService _service;
    private readonly Guid _testUserId = Guid.NewGuid();

    public TodoServiceTests()
    {
        _mockRepository = new Mock<ITodoRepository>();
        _service = new TodoService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllTodosAsync_ShouldCallRepository()
    {
        // Arrange
        var expectedTodos = new List<Todo>
        {
            new Todo { Id = Guid.NewGuid(), Text = "Test 1", UserId = _testUserId },
            new Todo { Id = Guid.NewGuid(), Text = "Test 2", UserId = _testUserId }
        };
        _mockRepository.Setup(r => r.GetAllAsync(_testUserId))
            .ReturnsAsync(expectedTodos);

        // Act
        var result = await _service.GetAllTodosAsync(_testUserId);

        // Assert
        result.Should().BeEquivalentTo(expectedTodos);
        _mockRepository.Verify(r => r.GetAllAsync(_testUserId), Times.Once);
    }

    [Fact]
    public async Task GetTodoByIdAsync_WhenTodoExists_ShouldReturnTodo()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var expectedTodo = new Todo { Id = todoId, Text = "Test", UserId = _testUserId };
        _mockRepository.Setup(r => r.GetByIdAsync(todoId, _testUserId))
            .ReturnsAsync(expectedTodo);

        // Act
        var result = await _service.GetTodoByIdAsync(todoId, _testUserId);

        // Assert
        result.Should().BeEquivalentTo(expectedTodo);
        _mockRepository.Verify(r => r.GetByIdAsync(todoId, _testUserId), Times.Once);
    }

    [Fact]
    public async Task GetTodoByIdAsync_WhenTodoDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(todoId, _testUserId))
            .ReturnsAsync((Todo?)null);

        // Act
        var result = await _service.GetTodoByIdAsync(todoId, _testUserId);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(todoId, _testUserId), Times.Once);
    }

    [Fact]
    public async Task CreateTodoAsync_ShouldCreateTodoWithCorrectDefaults()
    {
        // Arrange
        var text = "New todo";
        Todo? capturedTodo = null;
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Todo>()))
            .Callback<Todo>(t => capturedTodo = t)
            .ReturnsAsync((Todo t) => t);

        // Act
        var result = await _service.CreateTodoAsync(text, _testUserId);

        // Assert
        capturedTodo.Should().NotBeNull();
        capturedTodo!.Text.Should().Be(text);
        capturedTodo.UserId.Should().Be(_testUserId);
        capturedTodo.IsComplete.Should().BeFalse();
        capturedTodo.Id.Should().NotBe(Guid.Empty);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Todo>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTodoAsync_WhenTodoExists_ShouldUpdateTextOnly()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var existingTodo = new Todo
        {
            Id = todoId,
            Text = "Old text",
            IsComplete = false,
            UserId = _testUserId
        };
        _mockRepository.Setup(r => r.GetByIdAsync(todoId, _testUserId))
            .ReturnsAsync(existingTodo);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Todo>()))
            .ReturnsAsync((Todo t) => t);

        // Act
        var result = await _service.UpdateTodoAsync(todoId, "New text", null, _testUserId);

        // Assert
        result.Should().NotBeNull();
        result!.Text.Should().Be("New text");
        result.IsComplete.Should().BeFalse();
        result.UpdatedDate.Should().NotBeNull();
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Todo>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTodoAsync_WhenTodoExists_ShouldUpdateIsCompleteOnly()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var existingTodo = new Todo
        {
            Id = todoId,
            Text = "Test text",
            IsComplete = false,
            UserId = _testUserId
        };
        _mockRepository.Setup(r => r.GetByIdAsync(todoId, _testUserId))
            .ReturnsAsync(existingTodo);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Todo>()))
            .ReturnsAsync((Todo t) => t);

        // Act
        var result = await _service.UpdateTodoAsync(todoId, null, true, _testUserId);

        // Assert
        result.Should().NotBeNull();
        result!.Text.Should().Be("Test text");
        result.IsComplete.Should().BeTrue();
        result.UpdatedDate.Should().NotBeNull();
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Todo>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTodoAsync_WhenTodoExists_ShouldUpdateBothTextAndIsComplete()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var existingTodo = new Todo
        {
            Id = todoId,
            Text = "Old text",
            IsComplete = false,
            UserId = _testUserId
        };
        _mockRepository.Setup(r => r.GetByIdAsync(todoId, _testUserId))
            .ReturnsAsync(existingTodo);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Todo>()))
            .ReturnsAsync((Todo t) => t);

        // Act
        var result = await _service.UpdateTodoAsync(todoId, "New text", true, _testUserId);

        // Assert
        result.Should().NotBeNull();
        result!.Text.Should().Be("New text");
        result.IsComplete.Should().BeTrue();
        result.UpdatedDate.Should().NotBeNull();
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Todo>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTodoAsync_WhenTodoDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(todoId, _testUserId))
            .ReturnsAsync((Todo?)null);

        // Act
        var result = await _service.UpdateTodoAsync(todoId, "New text", true, _testUserId);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Todo>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTodoAsync_ShouldSetUpdatedDateToUtcNow()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var before = DateTime.UtcNow;
        var existingTodo = new Todo
        {
            Id = todoId,
            Text = "Test",
            IsComplete = false,
            UserId = _testUserId
        };
        _mockRepository.Setup(r => r.GetByIdAsync(todoId, _testUserId))
            .ReturnsAsync(existingTodo);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Todo>()))
            .ReturnsAsync((Todo t) => t);

        // Act
        var result = await _service.UpdateTodoAsync(todoId, "New text", null, _testUserId);
        var after = DateTime.UtcNow;

        // Assert
        result.Should().NotBeNull();
        result!.UpdatedDate.Should().NotBeNull();
        result.UpdatedDate!.Value.Should().BeOnOrAfter(before).And.BeOnOrBefore(after.AddSeconds(1));
    }

    [Fact]
    public async Task DeleteTodoAsync_WhenTodoExists_ShouldReturnTrue()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteAsync(todoId, _testUserId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteTodoAsync(todoId, _testUserId);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(r => r.DeleteAsync(todoId, _testUserId), Times.Once);
    }

    [Fact]
    public async Task DeleteTodoAsync_WhenTodoDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteAsync(todoId, _testUserId))
            .ReturnsAsync(false);

        // Act
        var result = await _service.DeleteTodoAsync(todoId, _testUserId);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(r => r.DeleteAsync(todoId, _testUserId), Times.Once);
    }
}
