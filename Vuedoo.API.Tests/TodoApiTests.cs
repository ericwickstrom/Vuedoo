using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Vuedoo.Domain.Entities;

namespace Vuedoo.API.Tests;

public class TodoApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TodoApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllTodos_ShouldReturn200_WithTodoList()
    {
        // Act
        var response = await _client.GetAsync("/api/todos");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todos = await response.Content.ReadFromJsonAsync<List<Todo>>();
        todos.Should().NotBeNull();
        todos!.Should().HaveCountGreaterThanOrEqualTo(5); // At least seed data
    }

    [Fact]
    public async Task GetAllTodos_ShouldReturnValidJsonArray()
    {
        // Act
        var response = await _client.GetAsync("/api/todos");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
        content.Should().StartWith("[");
        content.Should().EndWith("]");
    }

    [Fact]
    public async Task GetTodoById_WhenTodoExists_ShouldReturn200()
    {
        // Arrange
        var allTodosResponse = await _client.GetAsync("/api/todos");
        var allTodos = await allTodosResponse.Content.ReadFromJsonAsync<List<Todo>>();
        var existingTodoId = allTodos!.First().Id;

        // Act
        var response = await _client.GetAsync($"/api/todos/{existingTodoId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todo = await response.Content.ReadFromJsonAsync<Todo>();
        todo.Should().NotBeNull();
        todo!.Id.Should().Be(existingTodoId);
    }

    [Fact]
    public async Task GetTodoById_WhenTodoDoesNotExist_ShouldReturn404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/todos/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTodo_WithValidData_ShouldReturn201()
    {
        // Arrange
        var newTodo = new { Text = "Test create todo" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/todos", newTodo);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        var createdTodo = await response.Content.ReadFromJsonAsync<Todo>();
        createdTodo.Should().NotBeNull();
        createdTodo!.Text.Should().Be("Test create todo");
        createdTodo.IsComplete.Should().BeFalse();
        createdTodo.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CreateTodo_ShouldAddTodoToRepository()
    {
        // Arrange
        var newTodo = new { Text = "Verify addition" };

        // Act
        var createResponse = await _client.PostAsJsonAsync("/api/todos", newTodo);
        var createdTodo = await createResponse.Content.ReadFromJsonAsync<Todo>();

        var getAllResponse = await _client.GetAsync("/api/todos");
        var allTodos = await getAllResponse.Content.ReadFromJsonAsync<List<Todo>>();

        // Assert
        allTodos.Should().Contain(t => t.Id == createdTodo!.Id);
    }

    [Fact]
    public async Task CreateTodo_ShouldReturnLocationHeader()
    {
        // Arrange
        var newTodo = new { Text = "Check location header" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/todos", newTodo);
        var createdTodo = await response.Content.ReadFromJsonAsync<Todo>();

        // Assert
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain($"/api/todos/{createdTodo!.Id}");
    }

    [Fact]
    public async Task UpdateTodo_WithValidData_ShouldReturn200()
    {
        // Arrange
        var allTodosResponse = await _client.GetAsync("/api/todos");
        var allTodos = await allTodosResponse.Content.ReadFromJsonAsync<List<Todo>>();
        var todoToUpdate = allTodos!.First();
        var updateRequest = new { Text = "Updated text", IsComplete = true };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/todos/{todoToUpdate.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedTodo = await response.Content.ReadFromJsonAsync<Todo>();
        updatedTodo.Should().NotBeNull();
        updatedTodo!.Text.Should().Be("Updated text");
        updatedTodo.IsComplete.Should().BeTrue();
        updatedTodo.UpdatedDate.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateTodo_TextOnly_ShouldUpdateOnlyText()
    {
        // Arrange
        var allTodosResponse = await _client.GetAsync("/api/todos");
        var allTodos = await allTodosResponse.Content.ReadFromJsonAsync<List<Todo>>();
        var todoToUpdate = allTodos!.First();
        var originalIsComplete = todoToUpdate.IsComplete;
        var updateRequest = new { Text = "Only text updated", IsComplete = (bool?)null };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/todos/{todoToUpdate.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedTodo = await response.Content.ReadFromJsonAsync<Todo>();
        updatedTodo!.Text.Should().Be("Only text updated");
        updatedTodo.IsComplete.Should().Be(originalIsComplete);
    }

    [Fact]
    public async Task UpdateTodo_WhenTodoDoesNotExist_ShouldReturn404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateRequest = new { Text = "Updated", IsComplete = true };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/todos/{nonExistentId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTodo_WhenTodoExists_ShouldReturn204()
    {
        // Arrange
        var createResponse = await _client.PostAsJsonAsync("/api/todos", new { Text = "To be deleted" });
        var createdTodo = await createResponse.Content.ReadFromJsonAsync<Todo>();

        // Act
        var response = await _client.DeleteAsync($"/api/todos/{createdTodo!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTodo_ShouldRemoveTodoFromRepository()
    {
        // Arrange
        var createResponse = await _client.PostAsJsonAsync("/api/todos", new { Text = "Verify deletion" });
        var createdTodo = await createResponse.Content.ReadFromJsonAsync<Todo>();

        // Act
        await _client.DeleteAsync($"/api/todos/{createdTodo!.Id}");

        var getResponse = await _client.GetAsync($"/api/todos/{createdTodo.Id}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTodo_WhenTodoDoesNotExist_ShouldReturn404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/todos/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task EndToEndWorkflow_CreateUpdateDelete_ShouldWork()
    {
        // Create
        var createResponse = await _client.PostAsJsonAsync("/api/todos", new { Text = "E2E Test" });
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<Todo>();

        // Read
        var getResponse = await _client.GetAsync($"/api/todos/{created!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var retrieved = await getResponse.Content.ReadFromJsonAsync<Todo>();
        retrieved!.Text.Should().Be("E2E Test");

        // Update
        var updateResponse = await _client.PutAsJsonAsync($"/api/todos/{created.Id}",
            new { Text = "E2E Updated", IsComplete = true });
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await updateResponse.Content.ReadFromJsonAsync<Todo>();
        updated!.Text.Should().Be("E2E Updated");
        updated.IsComplete.Should().BeTrue();

        // Delete
        var deleteResponse = await _client.DeleteAsync($"/api/todos/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var verifyResponse = await _client.GetAsync($"/api/todos/{created.Id}");
        verifyResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
