using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Services.Todos.Interfaces;

namespace ToDoApp.Services.Todos
{
    public class TodoDeleteService : ITodoDeleteService
    {
        private readonly ILogger<TodoDeleteService> _logger;
        private readonly AppDbContext _context;

        public TodoDeleteService(AppDbContext context, ILogger<TodoDeleteService> logger)
        {
            _logger = logger;
            _context = context;
        }
        /// <summary>
        /// Deletes a Todo item by ID. Allows deletion by owner or admin.
        /// </summary>
        public async Task<bool> DeleteTodoAsync(int id, string userId, bool isAdmin)
        {
            _logger.LogInformation("Todo with Id {id} get delete from UserId {UserId}, isAdmin: {IsAdmin} ", id, userId, isAdmin);
            try
            {   
                var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == id);

                if (todo == null)
                {
                    _logger.LogWarning("Todo with Id {Id} not found.", id);
                    return false;
                }

                if (todo.UserId != userId && !isAdmin)
                    return false;
                
                _context.Todos.Remove(todo);
                await _context.SaveChangesAsync();   
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail in DeleteTodoAsync {id}", id);
                throw;
            }
        }
    }
}
