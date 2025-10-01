using Vuedoo.Domain.Entities;

namespace Vuedoo.Domain.Tests;

public class TodoTests
{
    [Fact]
    public void Todo_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var todo = new Todo();

        // Assert
        Assert.NotEqual(Guid.Empty, todo.Id);
        Assert.Equal(string.Empty, todo.Text);
        Assert.False(todo.IsComplete);
        Assert.NotEqual(default(DateTime), todo.CreatedDate);
        Assert.Null(todo.UpdatedDate);
        Assert.Equal(Guid.Empty, todo.UserId);
    }

    [Fact]
    public void Todo_ShouldAllowSettingProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var createdDate = DateTime.UtcNow.AddDays(-1);
        var updatedDate = DateTime.UtcNow;

        // Act
        var todo = new Todo
        {
            Id = id,
            Text = "Test todo",
            IsComplete = true,
            CreatedDate = createdDate,
            UpdatedDate = updatedDate,
            UserId = userId
        };

        // Assert
        Assert.Equal(id, todo.Id);
        Assert.Equal("Test todo", todo.Text);
        Assert.True(todo.IsComplete);
        Assert.Equal(createdDate, todo.CreatedDate);
        Assert.Equal(updatedDate, todo.UpdatedDate);
        Assert.Equal(userId, todo.UserId);
    }

    [Fact]
    public void Todo_CreatedDate_ShouldBeSetAutomatically()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var todo = new Todo();
        var after = DateTime.UtcNow;

        // Assert
        Assert.InRange(todo.CreatedDate, before.AddSeconds(-1), after.AddSeconds(1));
    }

    [Fact]
    public void Todo_Id_ShouldBeUniqueForEachInstance()
    {
        // Arrange & Act
        var todo1 = new Todo();
        var todo2 = new Todo();

        // Assert
        Assert.NotEqual(todo1.Id, todo2.Id);
    }
}
