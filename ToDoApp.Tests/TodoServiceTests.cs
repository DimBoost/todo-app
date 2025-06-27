using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Claims;
using ToDoApp.Data;
using ToDoApp.Models;
using ToDoApp.Models.Requests;
using ToDoApp.Services.Todos;

public class TodoServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;


        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetTodosAsync_ReturnsTodosForUser()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        
        // Test-Daten anlegen
        var userId = "user1";
        context.Todos.AddRange(
            new Todo { Id = 1, UserId = userId, Title = "Test Todo 1", IsCompleted = false },
            new Todo { Id = 2, UserId = "user2", Title = "Test Todo 2", IsCompleted = true }
        );
        await context.SaveChangesAsync();

        var logger = NullLogger<TodoService>.Instance;
        var service = new TodoService(context, logger);

        // Act
        //var (todos, total) = await service.GetTodosAsync(userId, isAdmin: false, null, null, null, null, 1, 10);

        var (todos, total) = await service.GetTodosAsync(new GetTodosRequest { UserId = userId});

        // Assert
        Assert.Single(todos);
        Assert.Equal(1, total);
        Assert.Equal("Test Todo 1", todos.First().Title);
    }

    [Fact]
    public async Task ToggleCompleteAsync_TogglesTodo_WhenUserIsOwner()
    {
        // Arrange
        var userId = "user1";
        var todo = new Todo { Id = 1, UserId = userId, Title = "Test", IsCompleted = false };
        
        var context = GetInMemoryDbContext();
        context.Todos.Add(todo);
        await context.SaveChangesAsync();

        var logger = NullLogger<TodoService>.Instance;
        var service = new TodoService(context, logger);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);
        var principalId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        // Act
        var result = await service.ToggleCompleteAsync(todo.Id, principalId, isAdmin: false);

        // Assert
        Assert.True(result);
        var updatedTodo = await context.Todos.FindAsync(todo.Id);
        Assert.True(updatedTodo.IsCompleted); // vorher false, jetzt true
    }

    [Fact]
    public async Task GetTodosAsync_ReturnsOnlyUserTodos_ForNormalUser()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Todos.AddRange(
            new Todo { Id = 1, UserId = "user1", Title = "Task A" },
            new Todo { Id = 2, UserId = "user2", Title = "Task B" }
        );
        await context.SaveChangesAsync();

        var service = new TodoService(context, NullLogger<TodoService>.Instance);
        //var (todos, total) = await service.GetTodosAsync("user1",false, null, null, null, null, 1, 10);
        var (todos, total) = await service.GetTodosAsync(new GetTodosRequest { UserId = "user1" });

        Assert.Single(todos);
        Assert.Equal("user1", todos[0].UserId);
    }

    [Fact]
    public async Task GetTodosAsync_FiltersBySearchQuery()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Todos.AddRange(
            new Todo { Id = 1, UserId = "user1", Title = "Projekt vorbereiten" },
            new Todo { Id = 2, UserId = "user1", Title = "Einkaufen" }
        );
        await context.SaveChangesAsync();

        var service = new TodoService(context, NullLogger<TodoService>.Instance);
        var (todos, total) = await service.GetTodosAsync(new GetTodosRequest { UserId = "user1", SearchQuery = "Projekt" });

        Assert.Single(todos);
        Assert.Contains("Projekt", todos[0].Title);
    }

    [Fact]
    public async Task GetTodosAsync_FiltersByCompletionStatus()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        context.Todos.AddRange(
            new Todo { Id = 1, UserId = "user1", Title = "A", IsCompleted = true },
            new Todo { Id = 2, UserId = "user2", Title = "B", IsCompleted = false }
        );
        await context.SaveChangesAsync();

        // Act
        var service = new TodoService(context, NullLogger<TodoService>.Instance);
        var (todos, total) = await service.GetTodosAsync(new GetTodosRequest { UserId = "user1", Filter = "completed" });
        // Assert
        Assert.Single(todos);
        Assert.True(todos[0].IsCompleted);
    }

    [Fact]
    public async Task GetTodosAsync_SearchesByTitle()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var userId = "user1";

        context.Todos.AddRange(
                new Todo { Id = 1, UserId = userId, Title = "Einkaufen gehen", IsCompleted = false },
                new Todo { Id = 2, UserId = userId, Title = "Sport machen", IsCompleted = false }
            );
        await context.SaveChangesAsync();

        var logger = NullLogger<TodoService>.Instance;
        var service = new TodoService(context, logger);

        var (todos, total) = await service.GetTodosAsync(new GetTodosRequest { UserId = userId ,SearchQuery = "Sport machen" });

        Assert.Single(todos);
        Assert.Equal("Sport machen", todos[0].Title);
    }
    // Prüfen ob User Admin ist
    [Fact]
    public async Task GetTodosAsync_ReturnsAllTodos_ForAdmin()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        // Users anlegen
        var user1 = new ApplicationUser { Id = "user1", UserName = "alice", Email = "alice@example.com", FirstName = "Alice", LastName = "Anderson" };
        var user2 = new ApplicationUser { Id = "user2", UserName = "bob", Email = "bob@example.com", FirstName = "Bob", LastName = "Brown" };


        var todo1 = new Todo { Id = 1, Title = "Task 1", IsCompleted = false, UserId = user1.Id, User = user1 };
        var todo2 = new Todo { Id = 2, Title = "Task 2", IsCompleted = true, UserId = user2.Id, User = user2 };

        context.Users.AddRange(user1, user2);
        context.Todos.AddRange(todo1, todo2);
        await context.SaveChangesAsync();

        var logger = NullLogger<TodoService>.Instance;
        var service = new TodoService(context, logger);
        
        var (todos, total) = await service.GetTodosAsync(new GetTodosRequest { UserId = "admin", IsAdmin = true});
        
        // Assert
        Assert.Equal(2, total);
        Assert.Contains(todos, t => t.Title.Contains("Task 2"));
    }

    [Fact]
    public async Task GetDeleteAsync_DeleteTodo()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        var todo = new Todo { Id = 1, Title = "Einkaufen gehen", IsCompleted = false, UserId = "test-user-id"};

        context.Todos.Add(todo);
        await context.SaveChangesAsync();

        var logger = NullLogger<TodoDeleteService>.Instance;
        var service = new TodoDeleteService(context, logger);

        var claims = new List<Claim>
        {
        new Claim(ClaimTypes.NameIdentifier, "test-user-id")
        };
        
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        // Act
        await service.DeleteTodoAsync(todo.Id, userId, isAdmin: false);

        // Assert
        var deleteTodo = await context.Todos.FindAsync(todo.Id);
        Assert.Null(deleteTodo);
    }
}
