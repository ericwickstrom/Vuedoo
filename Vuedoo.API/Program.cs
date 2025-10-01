using Vuedoo.Application.Services;
using Vuedoo.Domain.Repositories;
using Vuedoo.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register application services
builder.Services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();
builder.Services.AddScoped<TodoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Temporary user ID for development (until authentication is implemented)
var tempUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

// GET /api/todos - Get all todos
app.MapGet("/api/todos", async (TodoService todoService) =>
{
    var todos = await todoService.GetAllTodosAsync(tempUserId);
    return Results.Ok(todos);
})
.WithName("GetAllTodos")
.WithOpenApi();

// GET /api/todos/{id} - Get todo by ID
app.MapGet("/api/todos/{id:guid}", async (Guid id, TodoService todoService) =>
{
    var todo = await todoService.GetTodoByIdAsync(id, tempUserId);
    return todo is not null ? Results.Ok(todo) : Results.NotFound();
})
.WithName("GetTodoById")
.WithOpenApi();

// POST /api/todos - Create new todo
app.MapPost("/api/todos", async (CreateTodoRequest request, TodoService todoService) =>
{
    var todo = await todoService.CreateTodoAsync(request.Text, tempUserId);
    return Results.Created($"/api/todos/{todo.Id}", todo);
})
.WithName("CreateTodo")
.WithOpenApi();

// PUT /api/todos/{id} - Update todo
app.MapPut("/api/todos/{id:guid}", async (Guid id, UpdateTodoRequest request, TodoService todoService) =>
{
    var todo = await todoService.UpdateTodoAsync(id, request.Text, request.IsComplete, tempUserId);
    return todo is not null ? Results.Ok(todo) : Results.NotFound();
})
.WithName("UpdateTodo")
.WithOpenApi();

// DELETE /api/todos/{id} - Delete todo
app.MapDelete("/api/todos/{id:guid}", async (Guid id, TodoService todoService) =>
{
    var deleted = await todoService.DeleteTodoAsync(id, tempUserId);
    return deleted ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteTodo")
.WithOpenApi();

app.Run();

// DTOs
record CreateTodoRequest(string Text);
record UpdateTodoRequest(string? Text, bool? IsComplete);

// Make Program class public for testing
public partial class Program { }
