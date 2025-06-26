using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Models;
using ToDoApp.Services.Todos.Interfaces;

namespace ToDoApp.Services.Todos
{
    public class TodoCreateService : ITodoCreateService
    {

        private readonly ILogger<TodoCreateService> _logger;
        private readonly AppDbContext _dbContext;
        // Konstruktor: Holt sich den Datenbankkontext per Dependency Injection
        public TodoCreateService(AppDbContext dbContext, ILogger<TodoCreateService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // Erstellen der Logik zum Erstellen eines Users
        public async Task<bool> CreateTodoAsync(Todo todo, string userId)
        {
            _logger.LogInformation("Todo with Id get create with {todo}, {userId} ", todo, userId);

            todo.UserId = userId;

            todo.CreatedAt = DateTime.Now;

            // Max 15 Einträge möglich zu erstellen
            int numberOfTitelRows = await _dbContext.Todos
                            .Where(t => t.UserId == userId)
                            .CountAsync();

            if (numberOfTitelRows >= 15)
                return false;

            _dbContext.Todos.Add(todo);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
